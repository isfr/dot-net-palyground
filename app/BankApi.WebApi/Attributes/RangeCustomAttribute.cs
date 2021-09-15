using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BankApi.WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RangeCustomAttribute : ValidationAttribute
    {
        
        public object Minimum { get; private set; }

        public object Maximum { get; private set; }

        public Type OperandType { get; private set; }

        public RangeCustomAttribute(Type type, string minimum, string maximum)
        {
            this.Maximum = maximum;
            this.Minimum = minimum;
            this.OperandType = type;
        }

        private Func<object, object> Conversion { get; set; }

        public override object TypeId => base.TypeId;

        public override bool RequiresValidationContext => base.RequiresValidationContext;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return base.IsDefaultAttribute();
        }

        public override bool IsValid(object value)
        {
            // Validate our properties and create the conversion function
            this.SetupConversion();

            // Automatically pass if value is null or empty. RequiredAttribute should be used to assert a value is not empty.
            if (value == null)
            {
                return true;
            }
            string s = value as string;
            if (s != null && String.IsNullOrEmpty(s))
            {
                return true;
            }

            object convertedValue = null;

            try
            {
                convertedValue = this.Conversion(value);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

            IComparable min = (IComparable)this.Minimum;
            IComparable max = (IComparable)this.Maximum;
            return min.CompareTo(convertedValue) < 0 && max.CompareTo(convertedValue) >= 0;
        }

        public override bool Match(object obj)
        {
            return base.Match(obj);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }

        private void Initialize(IComparable minimum, IComparable maximum, Func<object, object> conversion)
        {
            if (minimum.CompareTo(maximum) > 0)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "The maximum value '{0}' must be greater than or equal to the minimum value '{1}'", maximum, minimum));
            }

            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Conversion = conversion;
        }

        private void SetupConversion()
        {
            if (this.Conversion == null)
            {
                object minimum = this.Minimum;
                object maximum = this.Maximum;

                if (minimum == null || maximum == null)
                {
                    throw new InvalidOperationException("The minimum and maximum values must be set.");
                }
                
                Type type = this.OperandType;
                if (type == null)
                {
                    throw new InvalidOperationException("The OperandType must be set when strings are used for minimum and maximum values.");
                }

                Type comparableType = typeof(IComparable);
                if (!comparableType.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException(
                        String.Format(
                            CultureInfo.CurrentCulture,
                            "The type {0} must implement {1}.",
                            type.FullName,
                            comparableType.FullName));
                }

                TypeConverter converter = TypeDescriptor.GetConverter(type);
                IComparable min = (IComparable)converter.ConvertFromString((string)minimum);
                IComparable max = (IComparable)converter.ConvertFromString((string)maximum);

                Func<object, object> conversion = value => (value != null && value.GetType() == type) ? value : converter.ConvertFrom(value);

                this.Initialize(min, max, conversion);
            }
        }
    }
}
