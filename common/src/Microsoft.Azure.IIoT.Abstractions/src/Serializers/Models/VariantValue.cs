﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Serializers {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;

    /// <summary>
    /// Represents primitive or structurally complex value
    /// </summary>
    public abstract class VariantValue : ICloneable, IConvertible, IComparable {

        /// <summary>
        /// Get type of value
        /// </summary>
        /// <inheritdoc/>
        public abstract VariantValueType Type { get; }

        /// <summary>
        /// Property names of object
        /// </summary>
        public abstract IEnumerable<string> Keys { get; }

        /// <summary>
        /// Values of array
        /// </summary>
        public abstract IEnumerable<VariantValue> Values { get; }

        /// <summary>
        /// Provide raw value or null
        /// </summary>
        public abstract object Value { get; }

        /// <inheritdoc/>
        public VariantValue this[string key] =>
            TryGetValue(key, out var result) ? result : Null();

        /// <inheritdoc/>
        public VariantValue this[int index] =>
            TryGetValue(index, out var result) ? result : null;

        /// <summary>
        /// Length of array
        /// </summary>
        public abstract int Count { get; }

        /// <inheritdoc/>
        public virtual TypeCode GetTypeCode() {
            if (this.IsNull()) {
                return TypeCode.Empty;
            }
            if (IsBoolean()) {
                return TypeCode.Boolean;
            }
            if (IsInteger()) {
                return TypeCode.Int64;
            }
            if (IsFloat()) {
                return TypeCode.Decimal;
            }
            if (IsDateTime()) {
                return TypeCode.DateTime;
            }
            if (IsObject()) {
                return TypeCode.Object;
            }
            return TypeCode.String;
        }

        /// <inheritdoc/>
        public bool ToBoolean(IFormatProvider provider) {
            return As<bool>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator bool(VariantValue value) =>
            value.As<bool>();
        /// <inheritdoc/>
        public static explicit operator bool?(VariantValue value) =>
            value.IsNull() ? (bool?)null : value.As<bool>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(bool value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(bool? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public byte ToByte(IFormatProvider provider) {
            return As<byte>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator byte(VariantValue value) =>
            value.As<byte>();
        /// <inheritdoc/>
        public static explicit operator byte?(VariantValue value) =>
            value.IsNull() ? (byte?)null : value.As<byte>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(byte value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(byte? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public char ToChar(IFormatProvider provider) {
            return As<char>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator char(VariantValue value) =>
            value.As<char>();
        /// <inheritdoc/>
        public static explicit operator char?(VariantValue value) =>
            value.IsNull() ? (char?)null : value.As<char>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(char value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(char? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public DateTime ToDateTime(IFormatProvider provider) {
            return As<DateTime>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator DateTime(VariantValue value) =>
            value.As<DateTime>();
        /// <inheritdoc/>
        public static explicit operator DateTime?(VariantValue value) =>
            value.IsNull() ? (DateTime?)null : value.As<DateTime>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(DateTime value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(DateTime? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public static explicit operator DateTimeOffset(VariantValue value) =>
            value.As<DateTimeOffset>();
        /// <inheritdoc/>
        public static explicit operator DateTimeOffset?(VariantValue value) =>
            value.IsNull() ? (DateTimeOffset?)null : value.As<DateTimeOffset>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(DateTimeOffset value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(DateTimeOffset? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public decimal ToDecimal(IFormatProvider provider) {
            return As<decimal>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator decimal(VariantValue value) =>
            value.As<decimal>();
        /// <inheritdoc/>
        public static explicit operator decimal?(VariantValue value) =>
            value.IsNull() ? (decimal?)null : value.As<decimal>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(decimal value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(decimal? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public double ToDouble(IFormatProvider provider) {
            return As<double>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator double(VariantValue value) =>
            value.As<double>();
        /// <inheritdoc/>
        public static explicit operator double?(VariantValue value) =>
            value.IsNull() ? (double?)null : value.As<double>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(double value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(double? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public short ToInt16(IFormatProvider provider) {
            return As<short>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator short(VariantValue value) =>
            value.As<short>();
        /// <inheritdoc/>
        public static explicit operator short?(VariantValue value) =>
            value.IsNull() ? (short?)null : value.As<short>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(short value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(short? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public int ToInt32(IFormatProvider provider) {
            return As<int>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator int(VariantValue value) =>
            value.As<int>();
        /// <inheritdoc/>
        public static explicit operator int?(VariantValue value) =>
            value.IsNull() ? (int?)null : value.As<int>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(int value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(int? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public long ToInt64(IFormatProvider provider) {
            return As<long>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator long(VariantValue value) =>
            value.As<long>();
        /// <inheritdoc/>
        public static explicit operator long?(VariantValue value) =>
            value.IsNull() ? (long?)null : value.As<long>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(long value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(long? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public ushort ToUInt16(IFormatProvider provider) {
            return As<ushort>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator ushort(VariantValue value) =>
            value.As<ushort>();
        /// <inheritdoc/>
        public static explicit operator ushort?(VariantValue value) =>
            value.IsNull() ? (ushort?)null : value.As<ushort>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(ushort value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(ushort? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public uint ToUInt32(IFormatProvider provider) {
            return As<uint>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator uint(VariantValue value) =>
            value.As<uint>();
        /// <inheritdoc/>
        public static explicit operator uint?(VariantValue value) =>
            value.IsNull() ? (uint?)null : value.As<uint>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(uint value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(uint? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public ulong ToUInt64(IFormatProvider provider) {
            return As<ulong>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator ulong(VariantValue value) =>
            value.As<ulong>();
        /// <inheritdoc/>
        public static explicit operator ulong?(VariantValue value) =>
            value.IsNull() ? (ulong?)null : value.As<ulong>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(ulong value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(ulong? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public sbyte ToSByte(IFormatProvider provider) {
            return As<sbyte>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator sbyte(VariantValue value) =>
            value.As<sbyte>();
        /// <inheritdoc/>
        public static explicit operator sbyte?(VariantValue value) =>
            value.IsNull() ? (sbyte?)null : value.As<sbyte>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(sbyte value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(sbyte? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public float ToSingle(IFormatProvider provider) {
            return As<float>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator float(VariantValue value) =>
            value.As<float>();
        /// <inheritdoc/>
        public static explicit operator float?(VariantValue value) =>
            value.IsNull() ? (float?)null : value.As<float>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(float value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(float? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public string ToString(IFormatProvider provider) {
            return As<string>(provider);
        }
        /// <inheritdoc/>
        public static explicit operator string(VariantValue value) =>
            value.As<string>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(string value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public static explicit operator byte[](VariantValue value) =>
            value.As<byte[]>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(byte[] value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public static explicit operator Guid(VariantValue value) =>
            value.As<Guid>();
        /// <inheritdoc/>
        public static explicit operator Guid?(VariantValue value) =>
            value.IsNull() ? (Guid?)null : value.As<Guid>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(Guid value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(Guid? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public static explicit operator TimeSpan(VariantValue value) =>
            value.As<TimeSpan>();
        /// <inheritdoc/>
        public static explicit operator TimeSpan?(VariantValue value) =>
            value.IsNull() ? (TimeSpan?)null : value.As<TimeSpan>();
        /// <inheritdoc/>
        public static implicit operator VariantValue(TimeSpan value) =>
            new PrimitiveValue(value);
        /// <inheritdoc/>
        public static implicit operator VariantValue(TimeSpan? value) =>
            new PrimitiveValue(value);

        /// <inheritdoc/>
        public abstract object ToType(Type conversionType, IFormatProvider provider);

        /// <inheritdoc/>
        public static bool operator ==(VariantValue left, VariantValue right) =>
            left?.Equals(right) ?? right?.Equals(null) ?? true;
        /// <inheritdoc/>
        public static bool operator !=(VariantValue left, VariantValue right) =>
            !(left == right);
        /// <inheritdoc/>
        public static bool operator >(VariantValue left, VariantValue right) =>
            left is null ? right != null : left.CompareTo(right) > 0;
        /// <inheritdoc/>
        public static bool operator <(VariantValue left, VariantValue right) =>
            left is null ? right is null : left.CompareTo(right) < 0;
        /// <inheritdoc/>
        public static bool operator >=(VariantValue left, VariantValue right) =>
            left is null ? right != null : left.CompareTo(right) >= 0;
        /// <inheritdoc/>
        public static bool operator <=(VariantValue left, VariantValue right) =>
            left is null ? right is null : left.CompareTo(right) <= 0;

        /// <inheritdoc/>
        public override bool Equals(object o) {
            if (o is VariantValue v) {
                return Comparer.Equals(this, v);
            }
            if (o is null) {
                return this.IsNull();
            }
            // Compare to non variant value
            if (TryEqualsValue(o, out var result)) {
                return result;
            }
            // Fallback to generic
            return VariantValueComparer.EqualValues(Value, o);
        }

        /// <inheritdoc/>
        public int CompareTo(object o) {
            if (o is VariantValue v) {
                return Comparer.Compare(this, v);
            }
            // Compare to non variant value
            if (TryCompareToValue(o, out var result)) {
                return result;
            }
            // Try non variant compare
            return VariantValueComparer.CompareValues(Value, o);
        }

        /// <inheritdoc/>
        public override string ToString() {
            return ToString(SerializeOption.None);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            return GetDeepHashCode();
        }

        /// <inheritdoc/>
        public object Clone() {
            return Copy();
        }

        /// <summary>
        /// Returns whether the token is a float type
        /// </summary>
        /// <returns></returns>
        public bool IsFloat() {
            return TryGetFloat(out _);
        }

        /// <summary>
        /// Returns whether the token is a integer type
        /// </summary>
        /// <returns></returns>
        public bool IsInteger() {
            return TryGetInteger(out _);
        }

        /// <summary>
        /// Returns whether the token is a duration type
        /// </summary>
        /// <returns></returns>
        public bool IsTimeSpan() {
            return TryGetTimeSpan(out _);
        }

        /// <summary>
        /// Returns whether the token is a date type
        /// </summary>
        /// <returns></returns>
        public bool IsDateTime() {
            return TryGetDateTime(out _);
        }

        /// <summary>
        /// Returns whether the token is a Guid type
        /// </summary>
        /// <returns></returns>
        public bool IsGuid() {
            return TryGetGuid(out _);
        }

        /// <summary>
        /// Returns whether the token is a boolean type
        /// </summary>
        /// <returns></returns>
        public bool IsBoolean() {
            return TryGetBoolean(out _);
        }

        /// <summary>
        /// Returns whether the token is a string type
        /// </summary>
        /// <returns></returns>
        public bool IsString() {
            return TryGetString(out _);
        }

        /// <summary>
        /// Returns whether the token is a array
        /// </summary>
        /// <returns></returns>
        public bool IsArray() {
            if (Type == VariantValueType.Array ||
                Type == VariantValueType.Bytes) {
                return true;
            }
            if (Value.GetType().IsArray || Count > 0) {
                return true;
            }
            if (IsObject()) {
                return false;
            }
            var s = (string)this;
            try {
                Convert.FromBase64String(s);
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// Returns whether the token is a object type
        /// </summary>
        /// <returns></returns>
        public bool IsObject() {
            if (Type == VariantValueType.Object || Keys.Any()) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns floating point value
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        protected bool TryGetFloat(out object o) {
            o = Value;
            if (Type != VariantValueType.Primitive) {
                return false;
            }
            switch (Value) {
                case int _:
                case uint _:
                case long _:
                case ulong _:
                case short _:
                case ushort _:
                case sbyte _:
                case byte _:
                case char _:
                    return true;
                case BigInteger b:
                    o = (double)b;
                    return true;
                case float _:
                case double _:
                case decimal _:
                    return true;
                case string s:
                    var result = decimal.TryParse(s, out var d1);
                    o = d1;
                    return result;
                case IConvertible c:
                    try {
                        o = c.ToDecimal(CultureInfo.InvariantCulture);
                        return true;
                    }
                    catch {
                        return false;
                    }
                default:
                    if (decimal.TryParse(ToString(), out var d2)) {
                        o = d2;
                        return true;
                    }
                    break;
            }
            try {
                o = As<double>();
                return true;
            }
            catch {
            }
            return false;
        }

        /// <summary>
        /// Returns whether the token is a float type
        /// </summary>
        /// <returns></returns>
        protected bool TryGetInteger(out object o) {
            o = Value;
            if (Type != VariantValueType.Primitive) {
                return false;
            }
            switch (Value) {
                case int _:
                case uint _:
                case long _:
                case ulong _:
                case short _:
                case ushort _:
                case sbyte _:
                case byte _:
                case char _:
                case BigInteger _:
                    return true;
                case string s:
                    var result = BigInteger.TryParse(s, out var b1);
                    o = b1;
                    return result;
                case decimal dec:
                    o = new BigInteger(dec);
                    return decimal.Floor(dec).Equals(dec);
                case float f:
                    o = new BigInteger(f);
                    return Math.Floor(f).Equals(f);
                case double d:
                    o = new BigInteger(d);
                    return Math.Floor(d).Equals(d);
                case IConvertible c:
                    try {
                        o = c.ToInt64(CultureInfo.InvariantCulture);
                        return true;
                    }
                    catch {
                        try {
                            o = c.ToUInt64(CultureInfo.InvariantCulture);
                            return true;
                        }
                        catch {
                            return false;
                        }
                    }
                default:
                    if (BigInteger.TryParse(ToString(), out var b2)) {
                        o = b2;
                        return true;
                    }
                    break;
            }
            try {
                o = As<BigInteger>();
                return true;
            }
            catch {
            }
            return false;
        }

        /// <summary>
        /// Returns whether the token is a float type
        /// </summary>
        /// <returns></returns>
        protected bool TryGetTimeSpan(out object o) {
            o = Value;
            if (Type != VariantValueType.Primitive) {
                return false;
            }
            switch (Value) {
                case TimeSpan _:
                    return true;
                case string s:
                    var result = TimeSpan.TryParse(s, out var ts1);
                    o = ts1;
                    return result;
                default:
                    if (TimeSpan.TryParse(ToString(), out var ts2)) {
                        o = ts2;
                        return true;
                    }
                    break;
            }
            try {
                o = As<TimeSpan>();
                return true;
            }
            catch {
            }
            return false;
        }

        /// <summary>
        /// Returns whether the token is a float type
        /// </summary>
        /// <returns></returns>
        protected bool TryGetDateTime(out object o) {
            o = Value;
            if (Type != VariantValueType.Primitive) {
                return false;
            }
            switch (Value) {
                case DateTime _:
                case DateTimeOffset _:
                    return true;
                case string s:
                    if (DateTime.TryParse(s, out var dt1)) {
                        o = dt1;
                        return true;
                    }
                    if (DateTimeOffset.TryParse(s, out var dto1)) {
                        o = dto1;
                        return true;
                    }
                    return false;
                case IConvertible c:
                    try {
                        o = c.ToDateTime(CultureInfo.InvariantCulture);
                        return true;
                    }
                    catch {
                        return false;
                    }
                default:
                    if (DateTime.TryParse(ToString(), out var dt2)) {
                        o = dt2;
                        return true;
                    }
                    if (DateTimeOffset.TryParse(ToString(), out var dto2)) {
                        o = dto2;
                        return true;
                    }
                    break;
            }
            try {
                o = As<DateTime>();
                return true;
            }
            catch {
            }
            return false;
        }

        /// <summary>
        /// Returns whether the token is a float type
        /// </summary>
        /// <returns></returns>
        protected bool TryGetBoolean(out object o) {
            o = Value;
            if (Type != VariantValueType.Primitive) {
                return false;
            }
            switch (Value) {
                case bool _:
                    return true;
                case string s:
                    var result = bool.TryParse(s, out var b1);
                    o = b1;
                    return result;
                case IConvertible c:
                    try {
                        o = c.ToBoolean(CultureInfo.InvariantCulture);
                        return true;
                    }
                    catch {
                        return false;
                    }
                default:
                    if (bool.TryParse(ToString(), out var b2)) {
                        o = b2;
                        return true;
                    }
                    break;
            }
            try {
                o = As<bool>();
                return true;
            }
            catch {
            }
            return false;
        }

        /// <summary>
        /// Returns whether the token is a float type
        /// </summary>
        /// <returns></returns>
        protected bool TryGetGuid(out object o) {
            o = Value;
            if (Type != VariantValueType.Primitive) {
                return false;
            }
            switch (Value) {
                case Guid _:
                    return true;
                case string s:
                    var result = Guid.TryParse(s, out var g1);
                    o = g1;
                    return result;
                default:
                    if (Guid.TryParse(ToString(), out var g2)) {
                        o = g2;
                        return true;
                    }
                    break;
            }
            try {
                o = As<Guid>();
                return true;
            }
            catch {
            }
            return false;
        }

        /// <summary>
        /// Returns whether the token is a float type
        /// </summary>
        /// <returns></returns>
        protected bool TryGetString(out object o) {
            o = Value;
            if (Type != VariantValueType.Primitive) {
                return false;
            }
            switch (Value) {
                case string s:
                    return true;
            }
            o = ToString();
            return true;
        }

        /// <summary>
        /// Convert value to typed value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>(IFormatProvider provider = null) {
            var typed = ToType(typeof(T), provider);
            return typed == null ? default : (T)typed;
        }

        /// <summary>
        /// Convert value to typed value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object As(Type type) {
            return ToType(type, null);
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public abstract string ToString(SerializeOption format);

        /// <summary>
        /// Update the value to the new value.
        /// </summary>
        /// <param name="value"></param>
        public abstract void Set(object value);

        /// <summary>
        /// Create value which is set to null.
        /// </summary>
        /// <returns></returns>
        protected abstract VariantValue Null();

        /// <summary>
        /// Clone this item or entire tree
        /// </summary>
        /// <returns></returns>
        public abstract VariantValue Copy(bool shallow = false);

        /// <summary>
        /// Get value for property
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public abstract bool TryGetValue(string key, out VariantValue value,
            StringComparison compare = StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Get value from array index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryGetValue(int index, out VariantValue value);

        /// <summary>
        /// Select value using path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract VariantValue SelectToken(string path);

        /// <summary>
        /// Compare to a non variant value object, e.g. the value of
        /// another variant.  This can be overridden.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="equality"></param>
        /// <returns></returns>
        protected virtual bool TryEqualsValue(object o, out bool equality) {
            equality = false;
            return false;
        }

        /// <summary>
        /// Try to compare equality with another variant.
        /// The implementation should return false if comparison
        /// is not possible and must not call equality test
        /// with value itself.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="equality"></param>
        /// <returns></returns>
        protected virtual bool TryEqualsVariant(VariantValue v, out bool equality) {
            equality = false;
            return false;
        }

        /// <summary>
        /// Compare value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool TryCompareToValue(object obj, out int result) {
            result = 0;
            return false;
        }

        /// <summary>
        /// Compare variant value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool TryCompareToVariantValue(VariantValue v,
            out int result) {
            result = 0;
            return false;
        }

        /// <summary>
        /// Equality comparer
        /// </summary>
        public static VariantValueComparer Comparer => new VariantValueComparer();

        /// <inheritdoc/>
        public class VariantValueComparer : IEqualityComparer<VariantValue>,
            IComparer<VariantValue> {

            /// <inheritdoc/>
            public bool Equals(VariantValue x, VariantValue y) {
                if (ReferenceEquals(x, y)) {
                    return true;
                }

                var yt = y?.Type ?? VariantValueType.Null;
                var xt = x?.Type ?? VariantValueType.Null;

                if (yt != xt) {
                    return false;
                }

                if (xt == VariantValueType.Null && yt == VariantValueType.Null) {
                    // If both undefined or null then they are the same
                    return true;
                }

                // Perform structural comparison
                switch (xt) {
                    case VariantValueType.Null:
                        return true;
                    case VariantValueType.Array:
                        if (y.Values.SequenceEqual(x.Values, Comparer)) {
                            return true;
                        }
                        return false;
                    case VariantValueType.Object:
                        var p1 = x.Keys.OrderBy(k => k).Select(k => x[k]);
                        var p2 = y.Keys.OrderBy(k => k).Select(k => y[k]);
                        if (p1.SequenceEqual(p2, Comparer)) {
                            return true;
                        }
                        return false;
                    case VariantValueType.Bytes:
                        if (y.ToString(SerializeOption.None) ==
                            x.ToString(SerializeOption.None)) {
                            return true;
                        }
                        break;
                    case VariantValueType.Primitive:
                        if (y.ToString(SerializeOption.None) ==
                            x.ToString(SerializeOption.None)) {
                            return true;
                        }
                        if (x.IsFloat() && y.IsFloat()) {
                            return x.As<double>() == y.As<double>()
                        }
                        // Compare values themselves using converter and comparison
                        if (EqualValues(x.Value, y.Value)) {
                            return true;
                        }
                        var converter = TypeDescriptor.GetConverter(typeof(T));
                        result = (T)converter.ConvertFrom(o);
                        if (y.ToString(SerializeOption.None) ==
                            x.ToString(SerializeOption.None)) {
                            return true;
                        }
                        break;
                }

                //
                // Try from both sides as one implementation might not be
                // able to understand how to handle the other.
                //
                if (x.TryEqualsVariant(y, out var xe)) {
                    if (!xe) {
                        return false;
                    }
                    return true;
                }
                if (y.TryEqualsVariant(x, out var ye)) {
                    if (!ye) {
                        return false;
                    }
                    return true;
                }

                return false;
            }

            /// <inheritdoc/>
            public int Compare(VariantValue x, VariantValue y) {
                if (Equals(x, y)) {
                    return 0;
                }
                if (x.TryCompareToVariantValue(y, out var rx)) {
                    return rx;
                }
                if (y.TryCompareToVariantValue(x, out var ry)) {
                    return ry > 0 ? -1 : ry < 0 ? 1 : 0;
                }
                var yo = y.IsNull() ? null : y.Value;
                if (x.TryCompareToValue(yo, out var rxv)) {
                    return rxv;
                }
                var xo = x.IsNull() ? null : x.Value;
                if (y.TryCompareToValue(xo, out var ryv)) {
                    return ryv > 0 ? -1 : ryv < 0 ? 1 : 0;
                }
                return CompareValues(xo, yo);
            }

            /// <inheritdoc/>
            public int GetHashCode(VariantValue v) {
                return v?.GetDeepHashCode() ?? 0;
            }

            /// <summary>
            /// Tries to compare equality of 2 values using convertible
            /// and comparable interfaces.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            internal static bool EqualValues(object x, object y) {
                if (ReferenceEquals(x, y)) {
                    return true;
                }

                if (x is null || y is null) {
                    return false;
                }

                if (y.Equals(x)) {
                    return true;
                }

                TryCast(x, out var sx, x.ToString());
                TryCast(y, out var sy, y.ToString());

                // Compare bytes
                TryCast<byte[]>(x, out var bx);
                TryCast<byte[]>(y, out var by);
                if (by == null && bx != null && sy != null) {
                    try { by = Convert.FromBase64String(sy); }
                    catch { }
                }
                if (bx == null && by != null && sx != null) {
                    try { bx = Convert.FromBase64String(sx); }
                    catch { }
                }
                if (bx != null && by != null) {
                    return bx.AsSpan().SequenceEqual(by);
                }

                // Compare guids
                if (TryCast<Guid>(x, out _) || TryCast<Guid>(y, out _)) {
                    return sx == sy;
                }
                // Compare Uris
                if (TryCast<Uri>(x, out _) || TryCast<Uri>(y, out _)) {
                    return sx == sy;
                }

                if (x is IConvertible co1) {
                    try {
                        var compare = co1.ToType(y.GetType(),
                            CultureInfo.InvariantCulture);
                        return compare.Equals(y);
                    }
                    catch {
                    }
                }
                if (y is IConvertible co2) {
                    try {
                        var compare = co2.ToType(x.GetType(),
                            CultureInfo.InvariantCulture);
                        return compare.Equals(x);
                    }
                    catch {
                    }
                }

                // Compare values through comparison operation
                return CompareValues(x, y, true) == 0;
            }

            /// <summary>
            /// Compare value
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            internal static int CompareValues(object x, object y) {
                return CompareValues(x, y, false);
            }

            /// <summary>
            /// Compare
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="noStringCompare"></param>
            /// <returns></returns>
            private static int CompareValues(object x, object y,
                bool noStringCompare) {

                TryCast(x, out var sx, x?.ToString());
                TryCast(y, out var sy, y?.ToString());

                // Compare timespans
                if (TryCast<TimeSpan>(y, out var tsy1) && sx != null &&
                    TimeSpan.TryParse(sx, out var tsx1)) {
                    return tsx1.CompareTo(tsy1);
                }
                if (TryCast<TimeSpan>(x, out var tsx2) && sx != null &&
                    TimeSpan.TryParse(sy, out var tsy2)) {
                    return tsx2.CompareTo(tsy2);
                }

                // Compare dates
                if (TryCast<DateTime>(y, out var dty1) && sx != null &&
                    DateTime.TryParse(sx, out var dtx1)) {
                    return dtx1.CompareTo(dty1);
                }
                if (TryCast<DateTime>(x, out var dtx2) && sx != null &&
                    DateTime.TryParse(sy, out var dty2)) {
                    return dtx2.CompareTo(dty2);
                }

                if (TryCast<DateTimeOffset>(y, out var dtoy1) && sx != null &&
                    DateTimeOffset.TryParse(sx, out var dtox1)) {
                    return dtox1.CompareTo(dtoy1);
                }
                if (TryCast<DateTimeOffset>(x, out var dtox2) && sx != null &&
                    DateTimeOffset.TryParse(sy, out var dtoy2)) {
                    return dtox2.CompareTo(dtoy2);
                }

                if (x is IComparable cv1 && y is IConvertible co1) {
                    try {
                        var compared = cv1.CompareTo(co1.ToType(x.GetType(),
                            CultureInfo.InvariantCulture));
                        return compared < 0 ? -1 : compared > 0 ? 1 : 0;
                    }
                    catch {
                    }
                }
                // Compare the other way around
                if (y is IComparable cv2 && x is IConvertible co2) {
                    try {
                        var compared = cv2.CompareTo(co2.ToType(x.GetType(),
                            CultureInfo.InvariantCulture));
                        return compared > 0 ? -1 : compared < 0 ? 1 : 0;
                    }
                    catch {
                    }
                }

                if (noStringCompare) {
                    return -1;
                }

                // Compare stringified version
                var s1 = sx ?? "null";
                var s2 = sy ?? "null";
                return s1.CompareTo(s2);
            }
        }

        /// <summary>
        /// Create hash code for this or entire tree.
        /// </summary>
        /// <returns></returns>
        private int GetDeepHashCode() {
            var hc = new HashCode();
            switch (Type) {
                case VariantValueType.Null:
                case VariantValueType.Undefined:
                    hc.Add(Type);
                    break;
                case VariantValueType.Bytes:
                    if (TryCast<byte[]>(Value, out var b)) {
                        hc.Add(Convert.ToBase64String(b));
                        break;
                    }
                    hc.Add(Value.ToString());
                    break;
                case VariantValueType.Primitive:
                    TryCast(Value, out var s, Value.ToString());
                    hc.Add(s);
                    break;
                case VariantValueType.Array:
                    foreach (var value in Values) {
                        hc.Add(value.GetDeepHashCode());
                    }
                    break;
                case VariantValueType.Object:
                    var p2 = Keys.OrderBy(k => k);
                    foreach (var k in p2) {
                        hc.Add(k);
                        hc.Add(this[k].GetDeepHashCode());
                    }
                    break;
                default:
                    hc.Add(Value);
                    break;
            }
            return hc.ToHashCode();
        }

        /// <summary>
        /// Represents a primitive value for assignment purposes
        /// </summary>
        internal sealed class PrimitiveValue : VariantValue {

            /// <inheritdoc/>
            public override VariantValueType Type { get; }

            /// <inheritdoc/>
            public override object Value { get; }

            /// <inheritdoc/>
            public override IEnumerable<string> Keys =>
                Enumerable.Empty<string>();

            /// <inheritdoc/>
            public override IEnumerable<VariantValue> Values =>
                Enumerable.Empty<VariantValue>();

            /// <inheritdoc/>
            public override int Count => 0;

            /// <summary>
            /// Clone
            /// </summary>
            /// <param name="value"></param>
            /// <param name="type"></param>
            internal PrimitiveValue(object value, VariantValueType type) {
                Value = value;
                Type = value is null ? VariantValueType.Null : type;
            }

            /// <inheritdoc/>
            public PrimitiveValue(string value) :
                this(value, VariantValueType.Primitive) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(byte[] value) :
                this(value, VariantValueType.Bytes) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(bool value) :
                this(value, VariantValueType.Boolean) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(byte value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(sbyte value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(short value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(ushort value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(int value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(uint value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(long value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(ulong value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(float value) :
                this(value, float.IsInfinity(value) ?
                    VariantValueType.Primitive : VariantValueType.Float) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(double value) :
                this(value, double.IsInfinity(value) ?
                    VariantValueType.Primitive : VariantValueType.Float) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(decimal value) :
                this(value, VariantValueType.Float) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(Guid value) :
                this(value, VariantValueType.Guid) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(DateTime value) :
                this(value.Kind == DateTimeKind.Local ?
                    value.ToUniversalTime() : value,
                    VariantValueType.UtcDateTime) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(DateTimeOffset value) :
                this(value.Offset != TimeSpan.Zero ?
                    value.ToUniversalTime() : value,
                    VariantValueType.UtcDateTime) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(TimeSpan value) :
                this(value, VariantValueType.TimeSpan) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(bool? value) :
                this(value, VariantValueType.Boolean) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(byte? value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(sbyte? value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(short? value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(ushort? value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(int? value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(uint? value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(long? value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(ulong? value) :
                this(value, VariantValueType.Integer) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(float? value) :
                this(value, value.HasValue && float.IsInfinity(value.Value) ?
                    VariantValueType.Primitive : VariantValueType.Float) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(double? value) :
                this(value, value.HasValue && double.IsInfinity(value.Value) ?
                    VariantValueType.Primitive : VariantValueType.Float) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(decimal? value) :
                this(value, VariantValueType.Float) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(Guid? value) :
                this(value, VariantValueType.Guid) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(DateTime? value) :
                this(!value.HasValue ? value :
                    value.Value.Kind == DateTimeKind.Local ?
                    value.Value.ToUniversalTime() : value.Value,
                    VariantValueType.UtcDateTime) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(DateTimeOffset? value) :
                this(!value.HasValue ? value :
                    value.Value.Offset != TimeSpan.Zero ?
                    value.Value.ToUniversalTime() : value.Value,
                    VariantValueType.UtcDateTime) {
            }

            /// <inheritdoc/>
            public PrimitiveValue(TimeSpan? value) :
                this(value, VariantValueType.TimeSpan) {
            }

            /// <inheritdoc/>
            public override VariantValue Copy(bool shallow = false) {
                return new PrimitiveValue(Value, Type);
            }

            /// <inheritdoc/>
            public override string ToString(SerializeOption format) {
                switch (Value) {
                    case null:
                        return "null";
                    case byte[] b:
                        return Convert.ToBase64String(b);
                    case string s:
                        return s;
                    default:
                        return Value.ToString();
                }
            }

            /// <inheritdoc/>
            public override object ToType(Type conversionType,
                IFormatProvider provider) {
                if (Value is null || Type == VariantValueType.Null) {
                    if (conversionType.IsValueType) {
                        return Activator.CreateInstance(conversionType);
                    }
                    return null;
                }
                if (conversionType.IsAssignableFrom(Value.GetType())) {
                    return Value;
                }
                if (Value is IConvertible c) {
                    return c.ToType(conversionType,
                        provider ?? CultureInfo.InvariantCulture);
                }
                var converter = TypeDescriptor.GetConverter(conversionType);
                return converter.ConvertFrom(Value);
            }

            /// <inheritdoc/>
            public override VariantValue SelectToken(string path) {
                throw new NotSupportedException("Not an object");
            }

            /// <inheritdoc/>
            public override void Set(object value) {
                throw new NotSupportedException("Not an object");
            }

            /// <inheritdoc/>
            public override bool TryGetValue(string key, out VariantValue value,
                StringComparison compare) {
                value = null;
                return false;
            }

            /// <inheritdoc/>
            public override bool TryGetValue(int index, out VariantValue value) {
                value = null;
                return false;
            }

            /// <inheritdoc/>
            protected override VariantValue Null() {
                return new PrimitiveValue(null, VariantValueType.Null);
            }
        }

        /// <summary>
        /// Helper to cast instead of pattern match
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="result"></param>
        /// <param name="defval"></param>
        /// <returns></returns>
        [DebuggerNonUserCode]
        protected static bool TryCast<T>(object o, out T result, T defval = default) {
            // Try pattern matching first
            if (o is T t) {
                result = t;
                return true;
            }
            try {
                try {
                    result = (T)Convert.ChangeType(o, typeof(T));
                }
                catch {
                    try {
                        var converter = TypeDescriptor.GetConverter(typeof(T));
                        result = (T)converter.ConvertFrom(o);
                    }
                    catch {
                        result = (T)o;
                    }
                }
                return true;
            }
            catch {
                result = defval;
                return false;
            }
        }

        /// <summary>
        /// Helper methods
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool DeepEquals(VariantValue x, VariantValue y) {
            return Comparer.Equals(x, y);
        }
    }
}