﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Serializers.MessagePack {
    using Microsoft.Azure.IIoT.Serializers;
    using Microsoft.Azure.IIoT.Exceptions;
    using System.Collections.Concurrent;
    using global::MessagePack.Formatters;
    using global::MessagePack.Resolvers;
    using global::MessagePack;
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using MsgPack = global::MessagePack.MessagePackSerializer;

    /// <summary>
    /// Message pack serializer
    /// </summary>
    public class MessagePackSerializer : IMessagePackSerializerOptionsProvider,
        ISerializer {

        /// <inheritdoc/>
        public string MimeType => ContentMimeType.MsgPack;

        /// <inheritdoc/>
        public Encoding ContentEncoding => null;

        /// <summary>
        /// Message pack options
        /// </summary>
        public MessagePackSerializerOptions Options { get; }

        /// <summary>
        /// Create serializer
        /// </summary>
        /// <param name="providers"></param>
        public MessagePackSerializer(
            IEnumerable<IMessagePackFormatterResolverProvider> providers = null) {
            // Create options
            var resolvers = new List<IFormatterResolver> {
                MessagePackVariantFormatterResolver.Instance
            };
            if (providers != null) {
                foreach (var provider in providers) {
                    var providedResolvers = provider.GetResolvers();
                    if (providedResolvers != null) {
                        resolvers.AddRange(providedResolvers);
                    }
                }
            }
            resolvers.Add(StandardResolver.Instance);
            Options = MessagePackSerializerOptions.Standard
                .WithSecurity(MessagePackSecurity.UntrustedData)
                .WithResolver(CompositeResolver.Create(resolvers.ToArray()))
                ;
        }

        /// <inheritdoc/>
        public object Deserialize(ReadOnlyMemory<byte> buffer, Type type) {
            try {
                return MsgPack.Deserialize(type, buffer, Options);
            }
            catch (MessagePackSerializationException ex) {
                throw new SerializerException(ex.Message, ex);
            }
        }

        /// <inheritdoc/>
        public void Serialize(IBufferWriter<byte> buffer, object o, SerializeOption format) {
            try {
                MsgPack.Serialize(buffer, o, Options);
            }
            catch (MessagePackSerializationException ex) {
                throw new SerializerException(ex.Message, ex);
            }
        }

        /// <inheritdoc/>
        public VariantValue Parse(ReadOnlyMemory<byte> buffer) {
            try {
                var o = MsgPack.Deserialize<object>(buffer, Options);
                if (o is VariantValue v) {
                    return v;
                }
                return new MessagePackVariantValue(o, Options, false);
            }
            catch (MessagePackSerializationException ex) {
                throw new SerializerException(ex.Message, ex);
            }
        }

        /// <inheritdoc/>
        public VariantValue FromObject(object o) {
            try {
                return new MessagePackVariantValue(o, Options, true);
            }
            catch (MessagePackSerializationException ex) {
                throw new SerializerException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Value wrapper
        /// </summary>
        internal class MessagePackVariantValue : VariantValue {

            /// <summary>
            /// Create value
            /// </summary>
            /// <param name="value"></param>
            /// <param name="serializer"></param>
            /// <param name="typed">Whether the object is the
            /// original type or the generated one</param>
            internal MessagePackVariantValue(object value,
                MessagePackSerializerOptions serializer, bool typed) {
                _options = serializer;
                _value = typed ? ToTypeLess(value) : value;
            }

            /// <inheritdoc/>
            protected override VariantValueType Type {
                get {
                    if (_value == null) {
                        return VariantValueType.Null;
                    }
                    if (_value is string s) {
                        return VariantValueType.Primitive;
                    }

                    var type = Value.GetType();
                    if (typeof(byte[]) == type) {
                        return VariantValueType.Bytes;
                    }
                    if (typeof(Guid) == type) {
                        return VariantValueType.Primitive;
                    }
                    if (typeof(Uri) == type) {
                        return VariantValueType.Primitive;
                    }
                    if (type.IsArray ||
                        typeof(IList<object>).IsAssignableFrom(type) ||
                        typeof(IEnumerable<object>).IsAssignableFrom(type)) {
                        return VariantValueType.Array;
                    }
                    if (typeof(IDictionary<object, object>).IsAssignableFrom(type)) {
                        return VariantValueType.Object;
                    }
                    if (type.IsGenericType &&
                        type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                        type = type.GetGenericArguments()[0];
                    }
                    if (typeof(bool) == type) {
                        return VariantValueType.Primitive;
                    }
                    if (typeof(DateTime) == type ||
                        typeof(DateTimeOffset) == type) {
                        return VariantValueType.Primitive;
                    }
                    if (typeof(TimeSpan) == type) {
                        return VariantValueType.Primitive;
                    }
                    if (typeof(uint) == type ||
                        typeof(int) == type ||
                        typeof(ulong) == type ||
                        typeof(long) == type ||
                        typeof(sbyte) == type ||
                        typeof(byte) == type ||
                        typeof(ushort) == type ||
                        typeof(short) == type ||
                        typeof(char) == type) {
                        return VariantValueType.Primitive;
                    }
                    if (typeof(float) == type ||
                        typeof(double) == type ||
                        typeof(BigInteger) == type ||
                        typeof(decimal) == type) {
                        return VariantValueType.Primitive;
                    }
                    if (type.GetProperties().Length > 0) {
                        return VariantValueType.Object;
                    }
                    return VariantValueType.Primitive;
                }
            }

            /// <inheritdoc/>
            public override object Value {
                get {
                    if (_value is object[] o && o.Length == 2 && o[0] is DateTime dt) {
                        var offset = Convert.ToInt64(o[1]);
                        if (offset == 0) {
                            return dt;
                        }
                        return new DateTimeOffset(dt, TimeSpan.FromTicks(offset));
                    }
                    return _value;
                }
            }

            /// <inheritdoc/>
            public override IEnumerable<string> Keys {
                get {
                    if (_value is IDictionary<object, object> o) {
                        return o.Keys.Select(p => p.ToString());
                    }
                    return Enumerable.Empty<string>();
                }
            }

            /// <inheritdoc/>
            public override IEnumerable<VariantValue> Values {
                get {
                    if (_value is IList<object> array) {
                        return array.Select(i =>
                            new MessagePackVariantValue(i, _options, false));
                    }
                    return Enumerable.Empty<VariantValue>();
                }
            }

            /// <inheritdoc/>
            public override int Count {
                get {
                    if (_value is IList<object> array) {
                        return array.Count;
                    }
                    return 0;
                }
            }

            /// <inheritdoc/>
            public override VariantValue Copy(bool shallow) {
                if (_value == null) {
                    return Null;
                }
                try {
                    return new MessagePackVariantValue(_value, _options, true);
                }
                catch (MessagePackSerializationException ex) {
                    throw new SerializerException(ex.Message, ex);
                }
            }

            /// <inheritdoc/>
            public override object ToType(Type type, IFormatProvider provider) {
                if (_value == null) {
                    return null;
                }
                var valueType = _value.GetType();
                if (type.IsAssignableFrom(valueType)) {
                    return _value;
                }
                try {
                    var buffer = new ArrayBufferWriter<byte>();
                    MsgPack.Serialize(buffer, _value, _options);
                    // Special case - convert byte array to buffer if not bin to begin.
                    if (type == typeof(byte[]) && valueType.IsArray) {
                        return ((IList<byte>)MsgPack.Deserialize(typeof(IList<byte>),
                            buffer.WrittenMemory, _options)).ToArray();
                    }
                    return MsgPack.Deserialize(type, buffer.WrittenMemory, _options);
                }
                catch (MessagePackSerializationException ex) {
                    throw new SerializerException(ex.Message, ex);
                }
            }

            /// <inheritdoc/>
            public override VariantValue SelectToken(string path) {
                throw new NotSupportedException("Path not supported");
            }

            /// <inheritdoc/>
            public override void Set(object value) {
                _value = value;
            }

            /// <inheritdoc/>
            public override bool TryGetProperty(string key, out VariantValue value,
                StringComparison compare) {
                if (_value is IDictionary<object, object> o) {
                    var success = o.FirstOrDefault(kv => key.Equals((string)kv.Key, compare));
                    if (success.Value != null) {
                        value = new MessagePackVariantValue(success.Value, _options, false);
                        return true;
                    }
                }
                value = Null;
                return false;
            }

            /// <inheritdoc/>
            public override bool TryGetElement(int index, out VariantValue value) {
                if (index >= 0 && _value is IList<object> o && index < o.Count) {
                    value = new MessagePackVariantValue(o[index], _options, false);
                    return true;
                }
                value = Null;
                return false;
            }

            /// <inheritdoc/>
            protected override VariantValue Null =>
                new MessagePackVariantValue(null, _options, false);

            /// <inheritdoc/>
            protected override bool TryCompareToValue(object o, out int result) {
                try {
                    if (_value is object[] a1 && a1.Length > 0 && a1[0] is IComparable c1 &&
                        _value is object[] a2 && a2.Length > 0 && a2[0] is IComparable c2) {
                        result = c1.CompareTo(c2);
                        return true;
                    }
                }
                catch {
                    // Try base
                }
                return base.TryCompareToValue(o, out result);
            }

            /// <inheritdoc/>
            protected override bool TryEqualsValue(object o, out bool equality) {
                try {
                    o = ToTypeLess(o);
                }
                catch {
                    return base.TryEqualsValue(o, out equality);
                }
                equality = DeepEquals(_value, o);
                return true;
            }

            /// <inheritdoc/>
            protected override bool TryEqualsVariant(VariantValue v, out bool equality) {
                if (v is MessagePackVariantValue packed) {
                    equality = DeepEquals(_value, packed._value);
                    return true;
                }

                // Special comparison to timespan
                var type = Type;
                if (v.IsTimeSpan) {
                    if (IsInteger || IsNumber) {
                        equality = v.Equals((VariantValue)TimeSpan.FromTicks(
                            Convert.ToInt64(_value)));
                        return true;
                    }
                }
                return base.TryEqualsVariant(v, out equality);
            }

            /// <summary>
            /// Compare tokens in more consistent fashion
            /// </summary>
            /// <param name="t1"></param>
            /// <param name="t2"></param>
            /// <returns></returns>
            internal bool DeepEquals(object t1, object t2) {
                if (t1 == null || t2 == null) {
                    return t1 == t2;
                }

                // Test object equals
                if (t1 is IDictionary<object, object> o1 &&
                    t2 is IDictionary<object, object> o2) {
                    // Compare properties in order of key
                    var props1 = o1.OrderBy(k => k.Key).Select(k => k.Value);
                    var props2 = o2.OrderBy(k => k.Key).Select(k => k.Value);
                    return props1.SequenceEqual(props2,
                        Compare.Using<object>((x, y) => DeepEquals(x, y)));
                }

                // Test array
                if (t1 is object[] c1 && t2 is object[] c2) {
                    return c1.SequenceEqual(c2,
                        Compare.Using<object>((x, y) => DeepEquals(x, y)));
                }

                // Test value equals
                if (t1.Equals(t2)) {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Convert to typeless object
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            internal object ToTypeLess(object value) {
                if (value == null) {
                    return null;
                }
                try {
                    var buffer = new ArrayBufferWriter<byte>();
                    MsgPack.Serialize(buffer, value, _options);
                    return MsgPack.Deserialize<object>(buffer.WrittenMemory, _options);
                }
                catch (MessagePackSerializationException ex) {
                    throw new SerializerException(ex.Message, ex);
                }
            }

            private readonly MessagePackSerializerOptions _options;
            private object _value;
        }

        /// <summary>
        /// Message pack resolver
        /// </summary>
        private class MessagePackVariantFormatterResolver : IFormatterResolver {

            public static readonly MessagePackVariantFormatterResolver Instance =
                new MessagePackVariantFormatterResolver();

            /// <inheritdoc/>
            public IMessagePackFormatter<T> GetFormatter<T>() {
                if (typeof(VariantValue).IsAssignableFrom(typeof(T))) {
                    return (IMessagePackFormatter<T>)GetVariantFormatter(typeof(T));
                }
                return null;
            }

            /// <summary>
            /// Create Message pack variant formater of specifed type
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            internal IMessagePackFormatter GetVariantFormatter(Type type) {
                return _cache.GetOrAdd(type,
                    (IMessagePackFormatter)Activator.CreateInstance(
                        typeof(MessagePackVariantFormatter<>).MakeGenericType(type)));
            }

            /// <summary>
            /// Variant formatter
            /// </summary>
            private sealed class MessagePackVariantFormatter<T> : IMessagePackFormatter<T>
                where T : VariantValue {

                /// <inheritdoc/>
                public void Serialize(ref MessagePackWriter writer, T value,
                    MessagePackSerializerOptions options) {
                    if (value is MessagePackVariantValue packed) {
                        MsgPack.Serialize(ref writer, packed.Value, options);
                    }
                    else if (value is VariantValue variant) {
                        if (variant.IsNull()) {
                            writer.WriteNil();
                        }
                        else if (variant.IsArray) {
                            writer.WriteArrayHeader(variant.Count);
                            foreach (var item in variant.Values) {
                                MsgPack.Serialize(ref writer, item, options);
                            }

                        }
                        else if (variant.IsObject) {
                            // Serialize objects as key value pairs
                            var dict = variant.Keys
                                .ToDictionary(k => k, k => variant[k]);
                            MsgPack.Serialize(ref writer, dict, options);
                        }
                        else if (variant.TryGetValue(out var primitive)) {
                            MsgPack.Serialize(ref writer, primitive, options);
                        }
                        else {
                            MsgPack.Serialize(ref writer, variant.Value, options);
                        }
                    }
                }

                /// <inheritdoc/>
                public T Deserialize(ref MessagePackReader reader,
                    MessagePackSerializerOptions options) {

                    // Read variant from reader
                    var o = MsgPack.Deserialize<object>(ref reader, options);
                    return new MessagePackVariantValue(o, options, false) as T;
                }
            }

            private readonly ConcurrentDictionary<Type, IMessagePackFormatter> _cache =
                new ConcurrentDictionary<Type, IMessagePackFormatter>();
        }
    }
}