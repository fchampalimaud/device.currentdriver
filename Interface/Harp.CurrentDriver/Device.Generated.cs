using Bonsai;
using Bonsai.Harp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace Harp.CurrentDriver
{
    /// <summary>
    /// Generates events and processes commands for the CurrentDriver device connected
    /// at the specified serial port.
    /// </summary>
    [Combinator(MethodName = nameof(Generate))]
    [WorkflowElementCategory(ElementCategory.Source)]
    [Description("Generates events and processes commands for the CurrentDriver device.")]
    public partial class Device : Bonsai.Harp.Device, INamedElement
    {
        /// <summary>
        /// Represents the unique identity class of the <see cref="CurrentDriver"/> device.
        /// This field is constant.
        /// </summary>
        public const int WhoAmI = 1282;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device() : base(WhoAmI) { }

        string INamedElement.Name => nameof(CurrentDriver);

        /// <summary>
        /// Gets a read-only mapping from address to register type.
        /// </summary>
        public static new IReadOnlyDictionary<int, Type> RegisterMap { get; } = new Dictionary<int, Type>
            (Bonsai.Harp.Device.RegisterMap.ToDictionary(entry => entry.Key, entry => entry.Value))
        {
            { 32, typeof(DigitalInputState) },
            { 33, typeof(OutputSet) },
            { 34, typeof(OutputClear) },
            { 35, typeof(OutputToggle) },
            { 36, typeof(OutputState) },
            { 37, typeof(Led0Current) },
            { 38, typeof(Led1Current) },
            { 39, typeof(Dac0Voltage) },
            { 40, typeof(Dac1Voltage) },
            { 41, typeof(LedEnable) },
            { 42, typeof(LedDisable) },
            { 43, typeof(LedState) },
            { 44, typeof(Led0MaxCurrent) },
            { 45, typeof(Led1MaxCurrent) },
            { 46, typeof(PulseEnable) },
            { 47, typeof(PulseDutyCycleLed0) },
            { 48, typeof(PulseDutyCycleLed1) },
            { 49, typeof(PulseFrequencyLed0) },
            { 50, typeof(PulseFrequencyLed1) },
            { 51, typeof(RampLed0) },
            { 52, typeof(RampLed1) },
            { 53, typeof(RampConfig) },
            { 54, typeof(Reserved0) },
            { 55, typeof(Reserved1) },
            { 56, typeof(Reserved2) },
            { 57, typeof(Reserved3) },
            { 58, typeof(EnableEvents) }
        };

        /// <summary>
        /// Gets the contents of the metadata file describing the <see cref="CurrentDriver"/>
        /// device registers.
        /// </summary>
        public static readonly string Metadata = GetDeviceMetadata();

        static string GetDeviceMetadata()
        {
            var deviceType = typeof(Device);
            using var metadataStream = deviceType.Assembly.GetManifestResourceStream($"{deviceType.Namespace}.device.yml");
            using var streamReader = new System.IO.StreamReader(metadataStream);
            return streamReader.ReadToEnd();
        }
    }

    /// <summary>
    /// Represents an operator that returns the contents of the metadata file
    /// describing the <see cref="CurrentDriver"/> device registers.
    /// </summary>
    [Description("Returns the contents of the metadata file describing the CurrentDriver device registers.")]
    public partial class GetMetadata : Source<string>
    {
        /// <summary>
        /// Returns an observable sequence with the contents of the metadata file
        /// describing the <see cref="CurrentDriver"/> device registers.
        /// </summary>
        /// <returns>
        /// A sequence with a single <see cref="string"/> object representing the
        /// contents of the metadata file.
        /// </returns>
        public override IObservable<string> Generate()
        {
            return Observable.Return(Device.Metadata);
        }
    }

    /// <summary>
    /// Represents an operator that groups the sequence of <see cref="CurrentDriver"/>" messages by register type.
    /// </summary>
    [Description("Groups the sequence of CurrentDriver messages by register type.")]
    public partial class GroupByRegister : Combinator<HarpMessage, IGroupedObservable<Type, HarpMessage>>
    {
        /// <summary>
        /// Groups an observable sequence of <see cref="CurrentDriver"/> messages
        /// by register type.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of observable groups, each of which corresponds to a unique
        /// <see cref="CurrentDriver"/> register.
        /// </returns>
        public override IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(message => Device.RegisterMap[message.Address]);
        }
    }

    /// <summary>
    /// Represents an operator that filters register-specific messages
    /// reported by the <see cref="CurrentDriver"/> device.
    /// </summary>
    /// <seealso cref="DigitalInputState"/>
    /// <seealso cref="OutputSet"/>
    /// <seealso cref="OutputClear"/>
    /// <seealso cref="OutputToggle"/>
    /// <seealso cref="OutputState"/>
    /// <seealso cref="Led0Current"/>
    /// <seealso cref="Led1Current"/>
    /// <seealso cref="Dac0Voltage"/>
    /// <seealso cref="Dac1Voltage"/>
    /// <seealso cref="LedEnable"/>
    /// <seealso cref="LedDisable"/>
    /// <seealso cref="LedState"/>
    /// <seealso cref="Led0MaxCurrent"/>
    /// <seealso cref="Led1MaxCurrent"/>
    /// <seealso cref="PulseEnable"/>
    /// <seealso cref="PulseDutyCycleLed0"/>
    /// <seealso cref="PulseDutyCycleLed1"/>
    /// <seealso cref="PulseFrequencyLed0"/>
    /// <seealso cref="PulseFrequencyLed1"/>
    /// <seealso cref="RampLed0"/>
    /// <seealso cref="RampLed1"/>
    /// <seealso cref="RampConfig"/>
    /// <seealso cref="EnableEvents"/>
    [XmlInclude(typeof(DigitalInputState))]
    [XmlInclude(typeof(OutputSet))]
    [XmlInclude(typeof(OutputClear))]
    [XmlInclude(typeof(OutputToggle))]
    [XmlInclude(typeof(OutputState))]
    [XmlInclude(typeof(Led0Current))]
    [XmlInclude(typeof(Led1Current))]
    [XmlInclude(typeof(Dac0Voltage))]
    [XmlInclude(typeof(Dac1Voltage))]
    [XmlInclude(typeof(LedEnable))]
    [XmlInclude(typeof(LedDisable))]
    [XmlInclude(typeof(LedState))]
    [XmlInclude(typeof(Led0MaxCurrent))]
    [XmlInclude(typeof(Led1MaxCurrent))]
    [XmlInclude(typeof(PulseEnable))]
    [XmlInclude(typeof(PulseDutyCycleLed0))]
    [XmlInclude(typeof(PulseDutyCycleLed1))]
    [XmlInclude(typeof(PulseFrequencyLed0))]
    [XmlInclude(typeof(PulseFrequencyLed1))]
    [XmlInclude(typeof(RampLed0))]
    [XmlInclude(typeof(RampLed1))]
    [XmlInclude(typeof(RampConfig))]
    [XmlInclude(typeof(EnableEvents))]
    [Description("Filters register-specific messages reported by the CurrentDriver device.")]
    public class FilterRegister : FilterRegisterBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegister"/> class.
        /// </summary>
        public FilterRegister()
        {
            Register = new DigitalInputState();
        }

        string INamedElement.Name
        {
            get => $"{nameof(CurrentDriver)}.{GetElementDisplayName(Register)}";
        }
    }

    /// <summary>
    /// Represents an operator which filters and selects specific messages
    /// reported by the CurrentDriver device.
    /// </summary>
    /// <seealso cref="DigitalInputState"/>
    /// <seealso cref="OutputSet"/>
    /// <seealso cref="OutputClear"/>
    /// <seealso cref="OutputToggle"/>
    /// <seealso cref="OutputState"/>
    /// <seealso cref="Led0Current"/>
    /// <seealso cref="Led1Current"/>
    /// <seealso cref="Dac0Voltage"/>
    /// <seealso cref="Dac1Voltage"/>
    /// <seealso cref="LedEnable"/>
    /// <seealso cref="LedDisable"/>
    /// <seealso cref="LedState"/>
    /// <seealso cref="Led0MaxCurrent"/>
    /// <seealso cref="Led1MaxCurrent"/>
    /// <seealso cref="PulseEnable"/>
    /// <seealso cref="PulseDutyCycleLed0"/>
    /// <seealso cref="PulseDutyCycleLed1"/>
    /// <seealso cref="PulseFrequencyLed0"/>
    /// <seealso cref="PulseFrequencyLed1"/>
    /// <seealso cref="RampLed0"/>
    /// <seealso cref="RampLed1"/>
    /// <seealso cref="RampConfig"/>
    /// <seealso cref="EnableEvents"/>
    [XmlInclude(typeof(DigitalInputState))]
    [XmlInclude(typeof(OutputSet))]
    [XmlInclude(typeof(OutputClear))]
    [XmlInclude(typeof(OutputToggle))]
    [XmlInclude(typeof(OutputState))]
    [XmlInclude(typeof(Led0Current))]
    [XmlInclude(typeof(Led1Current))]
    [XmlInclude(typeof(Dac0Voltage))]
    [XmlInclude(typeof(Dac1Voltage))]
    [XmlInclude(typeof(LedEnable))]
    [XmlInclude(typeof(LedDisable))]
    [XmlInclude(typeof(LedState))]
    [XmlInclude(typeof(Led0MaxCurrent))]
    [XmlInclude(typeof(Led1MaxCurrent))]
    [XmlInclude(typeof(PulseEnable))]
    [XmlInclude(typeof(PulseDutyCycleLed0))]
    [XmlInclude(typeof(PulseDutyCycleLed1))]
    [XmlInclude(typeof(PulseFrequencyLed0))]
    [XmlInclude(typeof(PulseFrequencyLed1))]
    [XmlInclude(typeof(RampLed0))]
    [XmlInclude(typeof(RampLed1))]
    [XmlInclude(typeof(RampConfig))]
    [XmlInclude(typeof(EnableEvents))]
    [XmlInclude(typeof(TimestampedDigitalInputState))]
    [XmlInclude(typeof(TimestampedOutputSet))]
    [XmlInclude(typeof(TimestampedOutputClear))]
    [XmlInclude(typeof(TimestampedOutputToggle))]
    [XmlInclude(typeof(TimestampedOutputState))]
    [XmlInclude(typeof(TimestampedLed0Current))]
    [XmlInclude(typeof(TimestampedLed1Current))]
    [XmlInclude(typeof(TimestampedDac0Voltage))]
    [XmlInclude(typeof(TimestampedDac1Voltage))]
    [XmlInclude(typeof(TimestampedLedEnable))]
    [XmlInclude(typeof(TimestampedLedDisable))]
    [XmlInclude(typeof(TimestampedLedState))]
    [XmlInclude(typeof(TimestampedLed0MaxCurrent))]
    [XmlInclude(typeof(TimestampedLed1MaxCurrent))]
    [XmlInclude(typeof(TimestampedPulseEnable))]
    [XmlInclude(typeof(TimestampedPulseDutyCycleLed0))]
    [XmlInclude(typeof(TimestampedPulseDutyCycleLed1))]
    [XmlInclude(typeof(TimestampedPulseFrequencyLed0))]
    [XmlInclude(typeof(TimestampedPulseFrequencyLed1))]
    [XmlInclude(typeof(TimestampedRampLed0))]
    [XmlInclude(typeof(TimestampedRampLed1))]
    [XmlInclude(typeof(TimestampedRampConfig))]
    [XmlInclude(typeof(TimestampedEnableEvents))]
    [Description("Filters and selects specific messages reported by the CurrentDriver device.")]
    public partial class Parse : ParseBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Register = new DigitalInputState();
        }

        string INamedElement.Name => $"{nameof(CurrentDriver)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents an operator which formats a sequence of values as specific
    /// CurrentDriver register messages.
    /// </summary>
    /// <seealso cref="DigitalInputState"/>
    /// <seealso cref="OutputSet"/>
    /// <seealso cref="OutputClear"/>
    /// <seealso cref="OutputToggle"/>
    /// <seealso cref="OutputState"/>
    /// <seealso cref="Led0Current"/>
    /// <seealso cref="Led1Current"/>
    /// <seealso cref="Dac0Voltage"/>
    /// <seealso cref="Dac1Voltage"/>
    /// <seealso cref="LedEnable"/>
    /// <seealso cref="LedDisable"/>
    /// <seealso cref="LedState"/>
    /// <seealso cref="Led0MaxCurrent"/>
    /// <seealso cref="Led1MaxCurrent"/>
    /// <seealso cref="PulseEnable"/>
    /// <seealso cref="PulseDutyCycleLed0"/>
    /// <seealso cref="PulseDutyCycleLed1"/>
    /// <seealso cref="PulseFrequencyLed0"/>
    /// <seealso cref="PulseFrequencyLed1"/>
    /// <seealso cref="RampLed0"/>
    /// <seealso cref="RampLed1"/>
    /// <seealso cref="RampConfig"/>
    /// <seealso cref="EnableEvents"/>
    [XmlInclude(typeof(DigitalInputState))]
    [XmlInclude(typeof(OutputSet))]
    [XmlInclude(typeof(OutputClear))]
    [XmlInclude(typeof(OutputToggle))]
    [XmlInclude(typeof(OutputState))]
    [XmlInclude(typeof(Led0Current))]
    [XmlInclude(typeof(Led1Current))]
    [XmlInclude(typeof(Dac0Voltage))]
    [XmlInclude(typeof(Dac1Voltage))]
    [XmlInclude(typeof(LedEnable))]
    [XmlInclude(typeof(LedDisable))]
    [XmlInclude(typeof(LedState))]
    [XmlInclude(typeof(Led0MaxCurrent))]
    [XmlInclude(typeof(Led1MaxCurrent))]
    [XmlInclude(typeof(PulseEnable))]
    [XmlInclude(typeof(PulseDutyCycleLed0))]
    [XmlInclude(typeof(PulseDutyCycleLed1))]
    [XmlInclude(typeof(PulseFrequencyLed0))]
    [XmlInclude(typeof(PulseFrequencyLed1))]
    [XmlInclude(typeof(RampLed0))]
    [XmlInclude(typeof(RampLed1))]
    [XmlInclude(typeof(RampConfig))]
    [XmlInclude(typeof(EnableEvents))]
    [Description("Formats a sequence of values as specific CurrentDriver register messages.")]
    public partial class Format : FormatBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class.
        /// </summary>
        public Format()
        {
            Register = new DigitalInputState();
        }

        string INamedElement.Name => $"{nameof(CurrentDriver)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents a register that reflects the state of DI digital lines.
    /// </summary>
    [Description("Reflects the state of DI digital lines")]
    public partial class DigitalInputState
    {
        /// <summary>
        /// Represents the address of the <see cref="DigitalInputState"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="DigitalInputState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="DigitalInputState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="DigitalInputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DigitalInputs GetPayload(HarpMessage message)
        {
            return (DigitalInputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="DigitalInputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalInputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((DigitalInputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="DigitalInputState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DigitalInputState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DigitalInputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="DigitalInputState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DigitalInputState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DigitalInputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// DigitalInputState register.
    /// </summary>
    /// <seealso cref="DigitalInputState"/>
    [Description("Filters and selects timestamped messages from the DigitalInputState register.")]
    public partial class TimestampedDigitalInputState
    {
        /// <summary>
        /// Represents the address of the <see cref="DigitalInputState"/> register. This field is constant.
        /// </summary>
        public const int Address = DigitalInputState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="DigitalInputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalInputs> GetPayload(HarpMessage message)
        {
            return DigitalInputState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that set the specified digital output lines.
    /// </summary>
    [Description("Set the specified digital output lines")]
    public partial class OutputSet
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputSet"/> register. This field is constant.
        /// </summary>
        public const int Address = 33;

        /// <summary>
        /// Represents the payload type of the <see cref="OutputSet"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="OutputSet"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OutputSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DigitalOutputs GetPayload(HarpMessage message)
        {
            return (DigitalOutputs)message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadUInt16();
            return Timestamped.Create((DigitalOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OutputSet"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputSet"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromUInt16(Address, messageType, (ushort)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OutputSet"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputSet"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OutputSet register.
    /// </summary>
    /// <seealso cref="OutputSet"/>
    [Description("Filters and selects timestamped messages from the OutputSet register.")]
    public partial class TimestampedOutputSet
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputSet"/> register. This field is constant.
        /// </summary>
        public const int Address = OutputSet.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OutputSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetPayload(HarpMessage message)
        {
            return OutputSet.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that clear the specified digital output lines.
    /// </summary>
    [Description("Clear the specified digital output lines")]
    public partial class OutputClear
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputClear"/> register. This field is constant.
        /// </summary>
        public const int Address = 34;

        /// <summary>
        /// Represents the payload type of the <see cref="OutputClear"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="OutputClear"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OutputClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DigitalOutputs GetPayload(HarpMessage message)
        {
            return (DigitalOutputs)message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadUInt16();
            return Timestamped.Create((DigitalOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OutputClear"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputClear"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromUInt16(Address, messageType, (ushort)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OutputClear"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputClear"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OutputClear register.
    /// </summary>
    /// <seealso cref="OutputClear"/>
    [Description("Filters and selects timestamped messages from the OutputClear register.")]
    public partial class TimestampedOutputClear
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputClear"/> register. This field is constant.
        /// </summary>
        public const int Address = OutputClear.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OutputClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetPayload(HarpMessage message)
        {
            return OutputClear.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that toggle the specified digital output lines.
    /// </summary>
    [Description("Toggle the specified digital output lines")]
    public partial class OutputToggle
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputToggle"/> register. This field is constant.
        /// </summary>
        public const int Address = 35;

        /// <summary>
        /// Represents the payload type of the <see cref="OutputToggle"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="OutputToggle"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OutputToggle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DigitalOutputs GetPayload(HarpMessage message)
        {
            return (DigitalOutputs)message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputToggle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadUInt16();
            return Timestamped.Create((DigitalOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OutputToggle"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputToggle"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromUInt16(Address, messageType, (ushort)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OutputToggle"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputToggle"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OutputToggle register.
    /// </summary>
    /// <seealso cref="OutputToggle"/>
    [Description("Filters and selects timestamped messages from the OutputToggle register.")]
    public partial class TimestampedOutputToggle
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputToggle"/> register. This field is constant.
        /// </summary>
        public const int Address = OutputToggle.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OutputToggle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetPayload(HarpMessage message)
        {
            return OutputToggle.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that write the state of all digital output lines.
    /// </summary>
    [Description("Write the state of all digital output lines")]
    public partial class OutputState
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputState"/> register. This field is constant.
        /// </summary>
        public const int Address = 36;

        /// <summary>
        /// Represents the payload type of the <see cref="OutputState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="OutputState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="OutputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DigitalOutputs GetPayload(HarpMessage message)
        {
            return (DigitalOutputs)message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadUInt16();
            return Timestamped.Create((DigitalOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OutputState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromUInt16(Address, messageType, (ushort)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OutputState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OutputState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DigitalOutputs value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OutputState register.
    /// </summary>
    /// <seealso cref="OutputState"/>
    [Description("Filters and selects timestamped messages from the OutputState register.")]
    public partial class TimestampedOutputState
    {
        /// <summary>
        /// Represents the address of the <see cref="OutputState"/> register. This field is constant.
        /// </summary>
        public const int Address = OutputState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OutputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetPayload(HarpMessage message)
        {
            return OutputState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configuration of current to drive LED 0 [0:1000] mA.
    /// </summary>
    [Description("Configuration of current to drive LED 0 [0:1000] mA")]
    public partial class Led0Current
    {
        /// <summary>
        /// Represents the address of the <see cref="Led0Current"/> register. This field is constant.
        /// </summary>
        public const int Address = 37;

        /// <summary>
        /// Represents the payload type of the <see cref="Led0Current"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Led0Current"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Led0Current"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Led0Current"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Led0Current"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Led0Current"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Led0Current"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Led0Current"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Led0Current register.
    /// </summary>
    /// <seealso cref="Led0Current"/>
    [Description("Filters and selects timestamped messages from the Led0Current register.")]
    public partial class TimestampedLed0Current
    {
        /// <summary>
        /// Represents the address of the <see cref="Led0Current"/> register. This field is constant.
        /// </summary>
        public const int Address = Led0Current.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Led0Current"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Led0Current.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configuration of current to drive LED 1 [0:1000] mA.
    /// </summary>
    [Description("Configuration of current to drive LED 1 [0:1000] mA")]
    public partial class Led1Current
    {
        /// <summary>
        /// Represents the address of the <see cref="Led1Current"/> register. This field is constant.
        /// </summary>
        public const int Address = 38;

        /// <summary>
        /// Represents the payload type of the <see cref="Led1Current"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Led1Current"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Led1Current"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Led1Current"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Led1Current"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Led1Current"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Led1Current"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Led1Current"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Led1Current register.
    /// </summary>
    /// <seealso cref="Led1Current"/>
    [Description("Filters and selects timestamped messages from the Led1Current register.")]
    public partial class TimestampedLed1Current
    {
        /// <summary>
        /// Represents the address of the <see cref="Led1Current"/> register. This field is constant.
        /// </summary>
        public const int Address = Led1Current.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Led1Current"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Led1Current.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configuration of DAC 0 voltage [0:5000] mV.
    /// </summary>
    [Description("Configuration of DAC 0 voltage [0:5000] mV")]
    public partial class Dac0Voltage
    {
        /// <summary>
        /// Represents the address of the <see cref="Dac0Voltage"/> register. This field is constant.
        /// </summary>
        public const int Address = 39;

        /// <summary>
        /// Represents the payload type of the <see cref="Dac0Voltage"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Dac0Voltage"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Dac0Voltage"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Dac0Voltage"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Dac0Voltage"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Dac0Voltage"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Dac0Voltage"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Dac0Voltage"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Dac0Voltage register.
    /// </summary>
    /// <seealso cref="Dac0Voltage"/>
    [Description("Filters and selects timestamped messages from the Dac0Voltage register.")]
    public partial class TimestampedDac0Voltage
    {
        /// <summary>
        /// Represents the address of the <see cref="Dac0Voltage"/> register. This field is constant.
        /// </summary>
        public const int Address = Dac0Voltage.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Dac0Voltage"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Dac0Voltage.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configuration of DAC 1 voltage [0:5000] mV.
    /// </summary>
    [Description("Configuration of DAC 1 voltage [0:5000] mV")]
    public partial class Dac1Voltage
    {
        /// <summary>
        /// Represents the address of the <see cref="Dac1Voltage"/> register. This field is constant.
        /// </summary>
        public const int Address = 40;

        /// <summary>
        /// Represents the payload type of the <see cref="Dac1Voltage"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Dac1Voltage"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Dac1Voltage"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Dac1Voltage"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Dac1Voltage"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Dac1Voltage"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Dac1Voltage"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Dac1Voltage"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Dac1Voltage register.
    /// </summary>
    /// <seealso cref="Dac1Voltage"/>
    [Description("Filters and selects timestamped messages from the Dac1Voltage register.")]
    public partial class TimestampedDac1Voltage
    {
        /// <summary>
        /// Represents the address of the <see cref="Dac1Voltage"/> register. This field is constant.
        /// </summary>
        public const int Address = Dac1Voltage.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Dac1Voltage"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Dac1Voltage.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable driver on the selected output.
    /// </summary>
    [Description("Enable driver on the selected output")]
    public partial class LedEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="LedEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = 41;

        /// <summary>
        /// Represents the payload type of the <see cref="LedEnable"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="LedEnable"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LedEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LedOutputs GetPayload(HarpMessage message)
        {
            return (LedOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LedEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((LedOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LedEnable"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LedEnable"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LedEnable"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LedEnable"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LedEnable register.
    /// </summary>
    /// <seealso cref="LedEnable"/>
    [Description("Filters and selects timestamped messages from the LedEnable register.")]
    public partial class TimestampedLedEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="LedEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = LedEnable.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LedEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetPayload(HarpMessage message)
        {
            return LedEnable.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that disable driver on the selected output.
    /// </summary>
    [Description("Disable driver on the selected output")]
    public partial class LedDisable
    {
        /// <summary>
        /// Represents the address of the <see cref="LedDisable"/> register. This field is constant.
        /// </summary>
        public const int Address = 42;

        /// <summary>
        /// Represents the payload type of the <see cref="LedDisable"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="LedDisable"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LedDisable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LedOutputs GetPayload(HarpMessage message)
        {
            return (LedOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LedDisable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((LedOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LedDisable"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LedDisable"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LedDisable"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LedDisable"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LedDisable register.
    /// </summary>
    /// <seealso cref="LedDisable"/>
    [Description("Filters and selects timestamped messages from the LedDisable register.")]
    public partial class TimestampedLedDisable
    {
        /// <summary>
        /// Represents the address of the <see cref="LedDisable"/> register. This field is constant.
        /// </summary>
        public const int Address = LedDisable.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LedDisable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetPayload(HarpMessage message)
        {
            return LedDisable.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that control the correspondent LED output.
    /// </summary>
    [Description("Control the correspondent LED output")]
    public partial class LedState
    {
        /// <summary>
        /// Represents the address of the <see cref="LedState"/> register. This field is constant.
        /// </summary>
        public const int Address = 43;

        /// <summary>
        /// Represents the payload type of the <see cref="LedState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="LedState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LedState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LedOutputs GetPayload(HarpMessage message)
        {
            return (LedOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LedState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((LedOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LedState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LedState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LedState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LedState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LedState register.
    /// </summary>
    /// <seealso cref="LedState"/>
    [Description("Filters and selects timestamped messages from the LedState register.")]
    public partial class TimestampedLedState
    {
        /// <summary>
        /// Represents the address of the <see cref="LedState"/> register. This field is constant.
        /// </summary>
        public const int Address = LedState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LedState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetPayload(HarpMessage message)
        {
            return LedState.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configuration of current to drive LED 0 [0:1000] mA.
    /// </summary>
    [Description("Configuration of current to drive LED 0 [0:1000] mA")]
    public partial class Led0MaxCurrent
    {
        /// <summary>
        /// Represents the address of the <see cref="Led0MaxCurrent"/> register. This field is constant.
        /// </summary>
        public const int Address = 44;

        /// <summary>
        /// Represents the payload type of the <see cref="Led0MaxCurrent"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Led0MaxCurrent"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Led0MaxCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Led0MaxCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Led0MaxCurrent"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Led0MaxCurrent"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Led0MaxCurrent"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Led0MaxCurrent"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Led0MaxCurrent register.
    /// </summary>
    /// <seealso cref="Led0MaxCurrent"/>
    [Description("Filters and selects timestamped messages from the Led0MaxCurrent register.")]
    public partial class TimestampedLed0MaxCurrent
    {
        /// <summary>
        /// Represents the address of the <see cref="Led0MaxCurrent"/> register. This field is constant.
        /// </summary>
        public const int Address = Led0MaxCurrent.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Led0MaxCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Led0MaxCurrent.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configuration of current to drive LED 1 [0:1000] mA.
    /// </summary>
    [Description("Configuration of current to drive LED 1 [0:1000] mA")]
    public partial class Led1MaxCurrent
    {
        /// <summary>
        /// Represents the address of the <see cref="Led1MaxCurrent"/> register. This field is constant.
        /// </summary>
        public const int Address = 45;

        /// <summary>
        /// Represents the payload type of the <see cref="Led1MaxCurrent"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.Float;

        /// <summary>
        /// Represents the length of the <see cref="Led1MaxCurrent"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Led1MaxCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static float GetPayload(HarpMessage message)
        {
            return message.GetPayloadSingle();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Led1MaxCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadSingle();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Led1MaxCurrent"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Led1MaxCurrent"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Led1MaxCurrent"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Led1MaxCurrent"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, float value)
        {
            return HarpMessage.FromSingle(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Led1MaxCurrent register.
    /// </summary>
    /// <seealso cref="Led1MaxCurrent"/>
    [Description("Filters and selects timestamped messages from the Led1MaxCurrent register.")]
    public partial class TimestampedLed1MaxCurrent
    {
        /// <summary>
        /// Represents the address of the <see cref="Led1MaxCurrent"/> register. This field is constant.
        /// </summary>
        public const int Address = Led1MaxCurrent.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Led1MaxCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<float> GetPayload(HarpMessage message)
        {
            return Led1MaxCurrent.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enables the pulse function for the specified output DACs/LEDs.
    /// </summary>
    [Description("Enables the pulse function for the specified output DACs/LEDs")]
    public partial class PulseEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = 46;

        /// <summary>
        /// Represents the payload type of the <see cref="PulseEnable"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PulseEnable"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PulseEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LedOutputs GetPayload(HarpMessage message)
        {
            return (LedOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PulseEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((LedOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PulseEnable"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseEnable"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PulseEnable"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseEnable"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PulseEnable register.
    /// </summary>
    /// <seealso cref="PulseEnable"/>
    [Description("Filters and selects timestamped messages from the PulseEnable register.")]
    public partial class TimestampedPulseEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = PulseEnable.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PulseEnable"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetPayload(HarpMessage message)
        {
            return PulseEnable.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the duty cycle of the output pulse from 1 to 100.
    /// </summary>
    [Description("Specifies the duty cycle of the output pulse from 1 to 100")]
    public partial class PulseDutyCycleLed0
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseDutyCycleLed0"/> register. This field is constant.
        /// </summary>
        public const int Address = 47;

        /// <summary>
        /// Represents the payload type of the <see cref="PulseDutyCycleLed0"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PulseDutyCycleLed0"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PulseDutyCycleLed0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PulseDutyCycleLed0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PulseDutyCycleLed0"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseDutyCycleLed0"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PulseDutyCycleLed0"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseDutyCycleLed0"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PulseDutyCycleLed0 register.
    /// </summary>
    /// <seealso cref="PulseDutyCycleLed0"/>
    [Description("Filters and selects timestamped messages from the PulseDutyCycleLed0 register.")]
    public partial class TimestampedPulseDutyCycleLed0
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseDutyCycleLed0"/> register. This field is constant.
        /// </summary>
        public const int Address = PulseDutyCycleLed0.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PulseDutyCycleLed0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return PulseDutyCycleLed0.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the duty cycle of the output pulse from 1 to 100.
    /// </summary>
    [Description("Specifies the duty cycle of the output pulse from 1 to 100")]
    public partial class PulseDutyCycleLed1
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseDutyCycleLed1"/> register. This field is constant.
        /// </summary>
        public const int Address = 48;

        /// <summary>
        /// Represents the payload type of the <see cref="PulseDutyCycleLed1"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PulseDutyCycleLed1"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PulseDutyCycleLed1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PulseDutyCycleLed1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PulseDutyCycleLed1"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseDutyCycleLed1"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PulseDutyCycleLed1"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseDutyCycleLed1"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PulseDutyCycleLed1 register.
    /// </summary>
    /// <seealso cref="PulseDutyCycleLed1"/>
    [Description("Filters and selects timestamped messages from the PulseDutyCycleLed1 register.")]
    public partial class TimestampedPulseDutyCycleLed1
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseDutyCycleLed1"/> register. This field is constant.
        /// </summary>
        public const int Address = PulseDutyCycleLed1.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PulseDutyCycleLed1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return PulseDutyCycleLed1.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the frequency of the output pulse in Hz.
    /// </summary>
    [Description("Specifies the frequency of the output pulse in Hz")]
    public partial class PulseFrequencyLed0
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseFrequencyLed0"/> register. This field is constant.
        /// </summary>
        public const int Address = 49;

        /// <summary>
        /// Represents the payload type of the <see cref="PulseFrequencyLed0"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PulseFrequencyLed0"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PulseFrequencyLed0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PulseFrequencyLed0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PulseFrequencyLed0"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseFrequencyLed0"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PulseFrequencyLed0"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseFrequencyLed0"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PulseFrequencyLed0 register.
    /// </summary>
    /// <seealso cref="PulseFrequencyLed0"/>
    [Description("Filters and selects timestamped messages from the PulseFrequencyLed0 register.")]
    public partial class TimestampedPulseFrequencyLed0
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseFrequencyLed0"/> register. This field is constant.
        /// </summary>
        public const int Address = PulseFrequencyLed0.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PulseFrequencyLed0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return PulseFrequencyLed0.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the frequency of the output pulse in Hz.
    /// </summary>
    [Description("Specifies the frequency of the output pulse in Hz")]
    public partial class PulseFrequencyLed1
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseFrequencyLed1"/> register. This field is constant.
        /// </summary>
        public const int Address = 50;

        /// <summary>
        /// Represents the payload type of the <see cref="PulseFrequencyLed1"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="PulseFrequencyLed1"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="PulseFrequencyLed1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="PulseFrequencyLed1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="PulseFrequencyLed1"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseFrequencyLed1"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="PulseFrequencyLed1"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="PulseFrequencyLed1"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// PulseFrequencyLed1 register.
    /// </summary>
    /// <seealso cref="PulseFrequencyLed1"/>
    [Description("Filters and selects timestamped messages from the PulseFrequencyLed1 register.")]
    public partial class TimestampedPulseFrequencyLed1
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseFrequencyLed1"/> register. This field is constant.
        /// </summary>
        public const int Address = PulseFrequencyLed1.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="PulseFrequencyLed1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return PulseFrequencyLed1.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
    /// </summary>
    [Description("Specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off")]
    public partial class RampLed0
    {
        /// <summary>
        /// Represents the address of the <see cref="RampLed0"/> register. This field is constant.
        /// </summary>
        public const int Address = 51;

        /// <summary>
        /// Represents the payload type of the <see cref="RampLed0"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="RampLed0"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="RampLed0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="RampLed0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="RampLed0"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RampLed0"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="RampLed0"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RampLed0"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// RampLed0 register.
    /// </summary>
    /// <seealso cref="RampLed0"/>
    [Description("Filters and selects timestamped messages from the RampLed0 register.")]
    public partial class TimestampedRampLed0
    {
        /// <summary>
        /// Represents the address of the <see cref="RampLed0"/> register. This field is constant.
        /// </summary>
        public const int Address = RampLed0.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="RampLed0"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return RampLed0.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
    /// </summary>
    [Description("Specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off")]
    public partial class RampLed1
    {
        /// <summary>
        /// Represents the address of the <see cref="RampLed1"/> register. This field is constant.
        /// </summary>
        public const int Address = 52;

        /// <summary>
        /// Represents the payload type of the <see cref="RampLed1"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="RampLed1"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="RampLed1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="RampLed1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="RampLed1"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RampLed1"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="RampLed1"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RampLed1"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// RampLed1 register.
    /// </summary>
    /// <seealso cref="RampLed1"/>
    [Description("Filters and selects timestamped messages from the RampLed1 register.")]
    public partial class TimestampedRampLed1
    {
        /// <summary>
        /// Represents the address of the <see cref="RampLed1"/> register. This field is constant.
        /// </summary>
        public const int Address = RampLed1.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="RampLed1"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return RampLed1.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies when the ramps are applied for each DAC/LED.
    /// </summary>
    [Description("Specifies when the ramps are applied for each DAC/LED")]
    public partial class RampConfig
    {
        /// <summary>
        /// Represents the address of the <see cref="RampConfig"/> register. This field is constant.
        /// </summary>
        public const int Address = 53;

        /// <summary>
        /// Represents the payload type of the <see cref="RampConfig"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="RampConfig"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="RampConfig"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LedRamps GetPayload(HarpMessage message)
        {
            return (LedRamps)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="RampConfig"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedRamps> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((LedRamps)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="RampConfig"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RampConfig"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LedRamps value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="RampConfig"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="RampConfig"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LedRamps value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// RampConfig register.
    /// </summary>
    /// <seealso cref="RampConfig"/>
    [Description("Filters and selects timestamped messages from the RampConfig register.")]
    public partial class TimestampedRampConfig
    {
        /// <summary>
        /// Represents the address of the <see cref="RampConfig"/> register. This field is constant.
        /// </summary>
        public const int Address = RampConfig.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="RampConfig"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedRamps> GetPayload(HarpMessage message)
        {
            return RampConfig.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that reserved.
    /// </summary>
    [Description("Reserved")]
    internal partial class Reserved0
    {
        /// <summary>
        /// Represents the address of the <see cref="Reserved0"/> register. This field is constant.
        /// </summary>
        public const int Address = 54;

        /// <summary>
        /// Represents the payload type of the <see cref="Reserved0"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Reserved0"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;
    }

    /// <summary>
    /// Represents a register that reserved.
    /// </summary>
    [Description("Reserved")]
    internal partial class Reserved1
    {
        /// <summary>
        /// Represents the address of the <see cref="Reserved1"/> register. This field is constant.
        /// </summary>
        public const int Address = 55;

        /// <summary>
        /// Represents the payload type of the <see cref="Reserved1"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Reserved1"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;
    }

    /// <summary>
    /// Represents a register that reserved.
    /// </summary>
    [Description("Reserved")]
    internal partial class Reserved2
    {
        /// <summary>
        /// Represents the address of the <see cref="Reserved2"/> register. This field is constant.
        /// </summary>
        public const int Address = 56;

        /// <summary>
        /// Represents the payload type of the <see cref="Reserved2"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Reserved2"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;
    }

    /// <summary>
    /// Represents a register that reserved.
    /// </summary>
    [Description("Reserved")]
    internal partial class Reserved3
    {
        /// <summary>
        /// Represents the address of the <see cref="Reserved3"/> register. This field is constant.
        /// </summary>
        public const int Address = 57;

        /// <summary>
        /// Represents the payload type of the <see cref="Reserved3"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="Reserved3"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;
    }

    /// <summary>
    /// Represents a register that specifies the active events in the device.
    /// </summary>
    [Description("Specifies the active events in the device")]
    public partial class EnableEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = 58;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableEvents"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableEvents"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static CurrentDriverEvents GetPayload(HarpMessage message)
        {
            return (CurrentDriverEvents)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<CurrentDriverEvents> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((CurrentDriverEvents)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableEvents"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableEvents"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, CurrentDriverEvents value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableEvents"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableEvents"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, CurrentDriverEvents value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableEvents register.
    /// </summary>
    /// <seealso cref="EnableEvents"/>
    [Description("Filters and selects timestamped messages from the EnableEvents register.")]
    public partial class TimestampedEnableEvents
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableEvents"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableEvents.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableEvents"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<CurrentDriverEvents> GetPayload(HarpMessage message)
        {
            return EnableEvents.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator which creates standard message payloads for the
    /// CurrentDriver device.
    /// </summary>
    /// <seealso cref="CreateDigitalInputStatePayload"/>
    /// <seealso cref="CreateOutputSetPayload"/>
    /// <seealso cref="CreateOutputClearPayload"/>
    /// <seealso cref="CreateOutputTogglePayload"/>
    /// <seealso cref="CreateOutputStatePayload"/>
    /// <seealso cref="CreateLed0CurrentPayload"/>
    /// <seealso cref="CreateLed1CurrentPayload"/>
    /// <seealso cref="CreateDac0VoltagePayload"/>
    /// <seealso cref="CreateDac1VoltagePayload"/>
    /// <seealso cref="CreateLedEnablePayload"/>
    /// <seealso cref="CreateLedDisablePayload"/>
    /// <seealso cref="CreateLedStatePayload"/>
    /// <seealso cref="CreateLed0MaxCurrentPayload"/>
    /// <seealso cref="CreateLed1MaxCurrentPayload"/>
    /// <seealso cref="CreatePulseEnablePayload"/>
    /// <seealso cref="CreatePulseDutyCycleLed0Payload"/>
    /// <seealso cref="CreatePulseDutyCycleLed1Payload"/>
    /// <seealso cref="CreatePulseFrequencyLed0Payload"/>
    /// <seealso cref="CreatePulseFrequencyLed1Payload"/>
    /// <seealso cref="CreateRampLed0Payload"/>
    /// <seealso cref="CreateRampLed1Payload"/>
    /// <seealso cref="CreateRampConfigPayload"/>
    /// <seealso cref="CreateEnableEventsPayload"/>
    [XmlInclude(typeof(CreateDigitalInputStatePayload))]
    [XmlInclude(typeof(CreateOutputSetPayload))]
    [XmlInclude(typeof(CreateOutputClearPayload))]
    [XmlInclude(typeof(CreateOutputTogglePayload))]
    [XmlInclude(typeof(CreateOutputStatePayload))]
    [XmlInclude(typeof(CreateLed0CurrentPayload))]
    [XmlInclude(typeof(CreateLed1CurrentPayload))]
    [XmlInclude(typeof(CreateDac0VoltagePayload))]
    [XmlInclude(typeof(CreateDac1VoltagePayload))]
    [XmlInclude(typeof(CreateLedEnablePayload))]
    [XmlInclude(typeof(CreateLedDisablePayload))]
    [XmlInclude(typeof(CreateLedStatePayload))]
    [XmlInclude(typeof(CreateLed0MaxCurrentPayload))]
    [XmlInclude(typeof(CreateLed1MaxCurrentPayload))]
    [XmlInclude(typeof(CreatePulseEnablePayload))]
    [XmlInclude(typeof(CreatePulseDutyCycleLed0Payload))]
    [XmlInclude(typeof(CreatePulseDutyCycleLed1Payload))]
    [XmlInclude(typeof(CreatePulseFrequencyLed0Payload))]
    [XmlInclude(typeof(CreatePulseFrequencyLed1Payload))]
    [XmlInclude(typeof(CreateRampLed0Payload))]
    [XmlInclude(typeof(CreateRampLed1Payload))]
    [XmlInclude(typeof(CreateRampConfigPayload))]
    [XmlInclude(typeof(CreateEnableEventsPayload))]
    [XmlInclude(typeof(CreateTimestampedDigitalInputStatePayload))]
    [XmlInclude(typeof(CreateTimestampedOutputSetPayload))]
    [XmlInclude(typeof(CreateTimestampedOutputClearPayload))]
    [XmlInclude(typeof(CreateTimestampedOutputTogglePayload))]
    [XmlInclude(typeof(CreateTimestampedOutputStatePayload))]
    [XmlInclude(typeof(CreateTimestampedLed0CurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedLed1CurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedDac0VoltagePayload))]
    [XmlInclude(typeof(CreateTimestampedDac1VoltagePayload))]
    [XmlInclude(typeof(CreateTimestampedLedEnablePayload))]
    [XmlInclude(typeof(CreateTimestampedLedDisablePayload))]
    [XmlInclude(typeof(CreateTimestampedLedStatePayload))]
    [XmlInclude(typeof(CreateTimestampedLed0MaxCurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedLed1MaxCurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedPulseEnablePayload))]
    [XmlInclude(typeof(CreateTimestampedPulseDutyCycleLed0Payload))]
    [XmlInclude(typeof(CreateTimestampedPulseDutyCycleLed1Payload))]
    [XmlInclude(typeof(CreateTimestampedPulseFrequencyLed0Payload))]
    [XmlInclude(typeof(CreateTimestampedPulseFrequencyLed1Payload))]
    [XmlInclude(typeof(CreateTimestampedRampLed0Payload))]
    [XmlInclude(typeof(CreateTimestampedRampLed1Payload))]
    [XmlInclude(typeof(CreateTimestampedRampConfigPayload))]
    [XmlInclude(typeof(CreateTimestampedEnableEventsPayload))]
    [Description("Creates standard message payloads for the CurrentDriver device.")]
    public partial class CreateMessage : CreateMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessage"/> class.
        /// </summary>
        public CreateMessage()
        {
            Payload = new CreateDigitalInputStatePayload();
        }

        string INamedElement.Name => $"{nameof(CurrentDriver)}.{GetElementDisplayName(Payload)}";
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that reflects the state of DI digital lines.
    /// </summary>
    [DisplayName("DigitalInputStatePayload")]
    [Description("Creates a message payload that reflects the state of DI digital lines.")]
    public partial class CreateDigitalInputStatePayload
    {
        /// <summary>
        /// Gets or sets the value that reflects the state of DI digital lines.
        /// </summary>
        [Description("The value that reflects the state of DI digital lines.")]
        public DigitalInputs DigitalInputState { get; set; }

        /// <summary>
        /// Creates a message payload for the DigitalInputState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DigitalInputs GetPayload()
        {
            return DigitalInputState;
        }

        /// <summary>
        /// Creates a message that reflects the state of DI digital lines.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the DigitalInputState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.DigitalInputState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that reflects the state of DI digital lines.
    /// </summary>
    [DisplayName("TimestampedDigitalInputStatePayload")]
    [Description("Creates a timestamped message payload that reflects the state of DI digital lines.")]
    public partial class CreateTimestampedDigitalInputStatePayload : CreateDigitalInputStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that reflects the state of DI digital lines.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the DigitalInputState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.DigitalInputState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that set the specified digital output lines.
    /// </summary>
    [DisplayName("OutputSetPayload")]
    [Description("Creates a message payload that set the specified digital output lines.")]
    public partial class CreateOutputSetPayload
    {
        /// <summary>
        /// Gets or sets the value that set the specified digital output lines.
        /// </summary>
        [Description("The value that set the specified digital output lines.")]
        public DigitalOutputs OutputSet { get; set; }

        /// <summary>
        /// Creates a message payload for the OutputSet register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DigitalOutputs GetPayload()
        {
            return OutputSet;
        }

        /// <summary>
        /// Creates a message that set the specified digital output lines.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OutputSet register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.OutputSet.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that set the specified digital output lines.
    /// </summary>
    [DisplayName("TimestampedOutputSetPayload")]
    [Description("Creates a timestamped message payload that set the specified digital output lines.")]
    public partial class CreateTimestampedOutputSetPayload : CreateOutputSetPayload
    {
        /// <summary>
        /// Creates a timestamped message that set the specified digital output lines.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OutputSet register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.OutputSet.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that clear the specified digital output lines.
    /// </summary>
    [DisplayName("OutputClearPayload")]
    [Description("Creates a message payload that clear the specified digital output lines.")]
    public partial class CreateOutputClearPayload
    {
        /// <summary>
        /// Gets or sets the value that clear the specified digital output lines.
        /// </summary>
        [Description("The value that clear the specified digital output lines.")]
        public DigitalOutputs OutputClear { get; set; }

        /// <summary>
        /// Creates a message payload for the OutputClear register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DigitalOutputs GetPayload()
        {
            return OutputClear;
        }

        /// <summary>
        /// Creates a message that clear the specified digital output lines.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OutputClear register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.OutputClear.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that clear the specified digital output lines.
    /// </summary>
    [DisplayName("TimestampedOutputClearPayload")]
    [Description("Creates a timestamped message payload that clear the specified digital output lines.")]
    public partial class CreateTimestampedOutputClearPayload : CreateOutputClearPayload
    {
        /// <summary>
        /// Creates a timestamped message that clear the specified digital output lines.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OutputClear register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.OutputClear.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that toggle the specified digital output lines.
    /// </summary>
    [DisplayName("OutputTogglePayload")]
    [Description("Creates a message payload that toggle the specified digital output lines.")]
    public partial class CreateOutputTogglePayload
    {
        /// <summary>
        /// Gets or sets the value that toggle the specified digital output lines.
        /// </summary>
        [Description("The value that toggle the specified digital output lines.")]
        public DigitalOutputs OutputToggle { get; set; }

        /// <summary>
        /// Creates a message payload for the OutputToggle register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DigitalOutputs GetPayload()
        {
            return OutputToggle;
        }

        /// <summary>
        /// Creates a message that toggle the specified digital output lines.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OutputToggle register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.OutputToggle.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that toggle the specified digital output lines.
    /// </summary>
    [DisplayName("TimestampedOutputTogglePayload")]
    [Description("Creates a timestamped message payload that toggle the specified digital output lines.")]
    public partial class CreateTimestampedOutputTogglePayload : CreateOutputTogglePayload
    {
        /// <summary>
        /// Creates a timestamped message that toggle the specified digital output lines.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OutputToggle register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.OutputToggle.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that write the state of all digital output lines.
    /// </summary>
    [DisplayName("OutputStatePayload")]
    [Description("Creates a message payload that write the state of all digital output lines.")]
    public partial class CreateOutputStatePayload
    {
        /// <summary>
        /// Gets or sets the value that write the state of all digital output lines.
        /// </summary>
        [Description("The value that write the state of all digital output lines.")]
        public DigitalOutputs OutputState { get; set; }

        /// <summary>
        /// Creates a message payload for the OutputState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DigitalOutputs GetPayload()
        {
            return OutputState;
        }

        /// <summary>
        /// Creates a message that write the state of all digital output lines.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OutputState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.OutputState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that write the state of all digital output lines.
    /// </summary>
    [DisplayName("TimestampedOutputStatePayload")]
    [Description("Creates a timestamped message payload that write the state of all digital output lines.")]
    public partial class CreateTimestampedOutputStatePayload : CreateOutputStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that write the state of all digital output lines.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OutputState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.OutputState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configuration of current to drive LED 0 [0:1000] mA.
    /// </summary>
    [DisplayName("Led0CurrentPayload")]
    [Description("Creates a message payload that configuration of current to drive LED 0 [0:1000] mA.")]
    public partial class CreateLed0CurrentPayload
    {
        /// <summary>
        /// Gets or sets the value that configuration of current to drive LED 0 [0:1000] mA.
        /// </summary>
        [Range(min: 0, max: 1000)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that configuration of current to drive LED 0 [0:1000] mA.")]
        public float Led0Current { get; set; } = 0F;

        /// <summary>
        /// Creates a message payload for the Led0Current register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Led0Current;
        }

        /// <summary>
        /// Creates a message that configuration of current to drive LED 0 [0:1000] mA.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Led0Current register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Led0Current.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configuration of current to drive LED 0 [0:1000] mA.
    /// </summary>
    [DisplayName("TimestampedLed0CurrentPayload")]
    [Description("Creates a timestamped message payload that configuration of current to drive LED 0 [0:1000] mA.")]
    public partial class CreateTimestampedLed0CurrentPayload : CreateLed0CurrentPayload
    {
        /// <summary>
        /// Creates a timestamped message that configuration of current to drive LED 0 [0:1000] mA.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Led0Current register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Led0Current.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configuration of current to drive LED 1 [0:1000] mA.
    /// </summary>
    [DisplayName("Led1CurrentPayload")]
    [Description("Creates a message payload that configuration of current to drive LED 1 [0:1000] mA.")]
    public partial class CreateLed1CurrentPayload
    {
        /// <summary>
        /// Gets or sets the value that configuration of current to drive LED 1 [0:1000] mA.
        /// </summary>
        [Range(min: 0, max: 1000)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that configuration of current to drive LED 1 [0:1000] mA.")]
        public float Led1Current { get; set; } = 0F;

        /// <summary>
        /// Creates a message payload for the Led1Current register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Led1Current;
        }

        /// <summary>
        /// Creates a message that configuration of current to drive LED 1 [0:1000] mA.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Led1Current register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Led1Current.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configuration of current to drive LED 1 [0:1000] mA.
    /// </summary>
    [DisplayName("TimestampedLed1CurrentPayload")]
    [Description("Creates a timestamped message payload that configuration of current to drive LED 1 [0:1000] mA.")]
    public partial class CreateTimestampedLed1CurrentPayload : CreateLed1CurrentPayload
    {
        /// <summary>
        /// Creates a timestamped message that configuration of current to drive LED 1 [0:1000] mA.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Led1Current register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Led1Current.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configuration of DAC 0 voltage [0:5000] mV.
    /// </summary>
    [DisplayName("Dac0VoltagePayload")]
    [Description("Creates a message payload that configuration of DAC 0 voltage [0:5000] mV.")]
    public partial class CreateDac0VoltagePayload
    {
        /// <summary>
        /// Gets or sets the value that configuration of DAC 0 voltage [0:5000] mV.
        /// </summary>
        [Range(min: 0, max: 5000)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that configuration of DAC 0 voltage [0:5000] mV.")]
        public float Dac0Voltage { get; set; } = 0F;

        /// <summary>
        /// Creates a message payload for the Dac0Voltage register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Dac0Voltage;
        }

        /// <summary>
        /// Creates a message that configuration of DAC 0 voltage [0:5000] mV.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Dac0Voltage register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Dac0Voltage.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configuration of DAC 0 voltage [0:5000] mV.
    /// </summary>
    [DisplayName("TimestampedDac0VoltagePayload")]
    [Description("Creates a timestamped message payload that configuration of DAC 0 voltage [0:5000] mV.")]
    public partial class CreateTimestampedDac0VoltagePayload : CreateDac0VoltagePayload
    {
        /// <summary>
        /// Creates a timestamped message that configuration of DAC 0 voltage [0:5000] mV.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Dac0Voltage register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Dac0Voltage.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configuration of DAC 1 voltage [0:5000] mV.
    /// </summary>
    [DisplayName("Dac1VoltagePayload")]
    [Description("Creates a message payload that configuration of DAC 1 voltage [0:5000] mV.")]
    public partial class CreateDac1VoltagePayload
    {
        /// <summary>
        /// Gets or sets the value that configuration of DAC 1 voltage [0:5000] mV.
        /// </summary>
        [Range(min: 0, max: 5000)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that configuration of DAC 1 voltage [0:5000] mV.")]
        public float Dac1Voltage { get; set; } = 0F;

        /// <summary>
        /// Creates a message payload for the Dac1Voltage register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Dac1Voltage;
        }

        /// <summary>
        /// Creates a message that configuration of DAC 1 voltage [0:5000] mV.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Dac1Voltage register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Dac1Voltage.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configuration of DAC 1 voltage [0:5000] mV.
    /// </summary>
    [DisplayName("TimestampedDac1VoltagePayload")]
    [Description("Creates a timestamped message payload that configuration of DAC 1 voltage [0:5000] mV.")]
    public partial class CreateTimestampedDac1VoltagePayload : CreateDac1VoltagePayload
    {
        /// <summary>
        /// Creates a timestamped message that configuration of DAC 1 voltage [0:5000] mV.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Dac1Voltage register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Dac1Voltage.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable driver on the selected output.
    /// </summary>
    [DisplayName("LedEnablePayload")]
    [Description("Creates a message payload that enable driver on the selected output.")]
    public partial class CreateLedEnablePayload
    {
        /// <summary>
        /// Gets or sets the value that enable driver on the selected output.
        /// </summary>
        [Description("The value that enable driver on the selected output.")]
        public LedOutputs LedEnable { get; set; }

        /// <summary>
        /// Creates a message payload for the LedEnable register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LedOutputs GetPayload()
        {
            return LedEnable;
        }

        /// <summary>
        /// Creates a message that enable driver on the selected output.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LedEnable register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.LedEnable.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable driver on the selected output.
    /// </summary>
    [DisplayName("TimestampedLedEnablePayload")]
    [Description("Creates a timestamped message payload that enable driver on the selected output.")]
    public partial class CreateTimestampedLedEnablePayload : CreateLedEnablePayload
    {
        /// <summary>
        /// Creates a timestamped message that enable driver on the selected output.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LedEnable register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.LedEnable.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that disable driver on the selected output.
    /// </summary>
    [DisplayName("LedDisablePayload")]
    [Description("Creates a message payload that disable driver on the selected output.")]
    public partial class CreateLedDisablePayload
    {
        /// <summary>
        /// Gets or sets the value that disable driver on the selected output.
        /// </summary>
        [Description("The value that disable driver on the selected output.")]
        public LedOutputs LedDisable { get; set; }

        /// <summary>
        /// Creates a message payload for the LedDisable register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LedOutputs GetPayload()
        {
            return LedDisable;
        }

        /// <summary>
        /// Creates a message that disable driver on the selected output.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LedDisable register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.LedDisable.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that disable driver on the selected output.
    /// </summary>
    [DisplayName("TimestampedLedDisablePayload")]
    [Description("Creates a timestamped message payload that disable driver on the selected output.")]
    public partial class CreateTimestampedLedDisablePayload : CreateLedDisablePayload
    {
        /// <summary>
        /// Creates a timestamped message that disable driver on the selected output.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LedDisable register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.LedDisable.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that control the correspondent LED output.
    /// </summary>
    [DisplayName("LedStatePayload")]
    [Description("Creates a message payload that control the correspondent LED output.")]
    public partial class CreateLedStatePayload
    {
        /// <summary>
        /// Gets or sets the value that control the correspondent LED output.
        /// </summary>
        [Description("The value that control the correspondent LED output.")]
        public LedOutputs LedState { get; set; }

        /// <summary>
        /// Creates a message payload for the LedState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LedOutputs GetPayload()
        {
            return LedState;
        }

        /// <summary>
        /// Creates a message that control the correspondent LED output.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LedState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.LedState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that control the correspondent LED output.
    /// </summary>
    [DisplayName("TimestampedLedStatePayload")]
    [Description("Creates a timestamped message payload that control the correspondent LED output.")]
    public partial class CreateTimestampedLedStatePayload : CreateLedStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that control the correspondent LED output.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LedState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.LedState.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configuration of current to drive LED 0 [0:1000] mA.
    /// </summary>
    [DisplayName("Led0MaxCurrentPayload")]
    [Description("Creates a message payload that configuration of current to drive LED 0 [0:1000] mA.")]
    public partial class CreateLed0MaxCurrentPayload
    {
        /// <summary>
        /// Gets or sets the value that configuration of current to drive LED 0 [0:1000] mA.
        /// </summary>
        [Range(min: 0, max: 1000)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that configuration of current to drive LED 0 [0:1000] mA.")]
        public float Led0MaxCurrent { get; set; } = 0F;

        /// <summary>
        /// Creates a message payload for the Led0MaxCurrent register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Led0MaxCurrent;
        }

        /// <summary>
        /// Creates a message that configuration of current to drive LED 0 [0:1000] mA.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Led0MaxCurrent register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Led0MaxCurrent.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configuration of current to drive LED 0 [0:1000] mA.
    /// </summary>
    [DisplayName("TimestampedLed0MaxCurrentPayload")]
    [Description("Creates a timestamped message payload that configuration of current to drive LED 0 [0:1000] mA.")]
    public partial class CreateTimestampedLed0MaxCurrentPayload : CreateLed0MaxCurrentPayload
    {
        /// <summary>
        /// Creates a timestamped message that configuration of current to drive LED 0 [0:1000] mA.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Led0MaxCurrent register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Led0MaxCurrent.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configuration of current to drive LED 1 [0:1000] mA.
    /// </summary>
    [DisplayName("Led1MaxCurrentPayload")]
    [Description("Creates a message payload that configuration of current to drive LED 1 [0:1000] mA.")]
    public partial class CreateLed1MaxCurrentPayload
    {
        /// <summary>
        /// Gets or sets the value that configuration of current to drive LED 1 [0:1000] mA.
        /// </summary>
        [Range(min: 0, max: 1000)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that configuration of current to drive LED 1 [0:1000] mA.")]
        public float Led1MaxCurrent { get; set; } = 0F;

        /// <summary>
        /// Creates a message payload for the Led1MaxCurrent register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public float GetPayload()
        {
            return Led1MaxCurrent;
        }

        /// <summary>
        /// Creates a message that configuration of current to drive LED 1 [0:1000] mA.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Led1MaxCurrent register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Led1MaxCurrent.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configuration of current to drive LED 1 [0:1000] mA.
    /// </summary>
    [DisplayName("TimestampedLed1MaxCurrentPayload")]
    [Description("Creates a timestamped message payload that configuration of current to drive LED 1 [0:1000] mA.")]
    public partial class CreateTimestampedLed1MaxCurrentPayload : CreateLed1MaxCurrentPayload
    {
        /// <summary>
        /// Creates a timestamped message that configuration of current to drive LED 1 [0:1000] mA.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Led1MaxCurrent register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Led1MaxCurrent.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enables the pulse function for the specified output DACs/LEDs.
    /// </summary>
    [DisplayName("PulseEnablePayload")]
    [Description("Creates a message payload that enables the pulse function for the specified output DACs/LEDs.")]
    public partial class CreatePulseEnablePayload
    {
        /// <summary>
        /// Gets or sets the value that enables the pulse function for the specified output DACs/LEDs.
        /// </summary>
        [Description("The value that enables the pulse function for the specified output DACs/LEDs.")]
        public LedOutputs PulseEnable { get; set; }

        /// <summary>
        /// Creates a message payload for the PulseEnable register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LedOutputs GetPayload()
        {
            return PulseEnable;
        }

        /// <summary>
        /// Creates a message that enables the pulse function for the specified output DACs/LEDs.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PulseEnable register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.PulseEnable.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enables the pulse function for the specified output DACs/LEDs.
    /// </summary>
    [DisplayName("TimestampedPulseEnablePayload")]
    [Description("Creates a timestamped message payload that enables the pulse function for the specified output DACs/LEDs.")]
    public partial class CreateTimestampedPulseEnablePayload : CreatePulseEnablePayload
    {
        /// <summary>
        /// Creates a timestamped message that enables the pulse function for the specified output DACs/LEDs.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PulseEnable register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.PulseEnable.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the duty cycle of the output pulse from 1 to 100.
    /// </summary>
    [DisplayName("PulseDutyCycleLed0Payload")]
    [Description("Creates a message payload that specifies the duty cycle of the output pulse from 1 to 100.")]
    public partial class CreatePulseDutyCycleLed0Payload
    {
        /// <summary>
        /// Gets or sets the value that specifies the duty cycle of the output pulse from 1 to 100.
        /// </summary>
        [Range(min: 1, max: 100)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that specifies the duty cycle of the output pulse from 1 to 100.")]
        public byte PulseDutyCycleLed0 { get; set; } = 1;

        /// <summary>
        /// Creates a message payload for the PulseDutyCycleLed0 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return PulseDutyCycleLed0;
        }

        /// <summary>
        /// Creates a message that specifies the duty cycle of the output pulse from 1 to 100.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PulseDutyCycleLed0 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.PulseDutyCycleLed0.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the duty cycle of the output pulse from 1 to 100.
    /// </summary>
    [DisplayName("TimestampedPulseDutyCycleLed0Payload")]
    [Description("Creates a timestamped message payload that specifies the duty cycle of the output pulse from 1 to 100.")]
    public partial class CreateTimestampedPulseDutyCycleLed0Payload : CreatePulseDutyCycleLed0Payload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the duty cycle of the output pulse from 1 to 100.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PulseDutyCycleLed0 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.PulseDutyCycleLed0.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the duty cycle of the output pulse from 1 to 100.
    /// </summary>
    [DisplayName("PulseDutyCycleLed1Payload")]
    [Description("Creates a message payload that specifies the duty cycle of the output pulse from 1 to 100.")]
    public partial class CreatePulseDutyCycleLed1Payload
    {
        /// <summary>
        /// Gets or sets the value that specifies the duty cycle of the output pulse from 1 to 100.
        /// </summary>
        [Range(min: 1, max: 100)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that specifies the duty cycle of the output pulse from 1 to 100.")]
        public byte PulseDutyCycleLed1 { get; set; } = 1;

        /// <summary>
        /// Creates a message payload for the PulseDutyCycleLed1 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return PulseDutyCycleLed1;
        }

        /// <summary>
        /// Creates a message that specifies the duty cycle of the output pulse from 1 to 100.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PulseDutyCycleLed1 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.PulseDutyCycleLed1.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the duty cycle of the output pulse from 1 to 100.
    /// </summary>
    [DisplayName("TimestampedPulseDutyCycleLed1Payload")]
    [Description("Creates a timestamped message payload that specifies the duty cycle of the output pulse from 1 to 100.")]
    public partial class CreateTimestampedPulseDutyCycleLed1Payload : CreatePulseDutyCycleLed1Payload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the duty cycle of the output pulse from 1 to 100.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PulseDutyCycleLed1 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.PulseDutyCycleLed1.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the frequency of the output pulse in Hz.
    /// </summary>
    [DisplayName("PulseFrequencyLed0Payload")]
    [Description("Creates a message payload that specifies the frequency of the output pulse in Hz.")]
    public partial class CreatePulseFrequencyLed0Payload
    {
        /// <summary>
        /// Gets or sets the value that specifies the frequency of the output pulse in Hz.
        /// </summary>
        [Range(min: 1, max: long.MaxValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that specifies the frequency of the output pulse in Hz.")]
        public byte PulseFrequencyLed0 { get; set; } = 1;

        /// <summary>
        /// Creates a message payload for the PulseFrequencyLed0 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return PulseFrequencyLed0;
        }

        /// <summary>
        /// Creates a message that specifies the frequency of the output pulse in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PulseFrequencyLed0 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.PulseFrequencyLed0.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the frequency of the output pulse in Hz.
    /// </summary>
    [DisplayName("TimestampedPulseFrequencyLed0Payload")]
    [Description("Creates a timestamped message payload that specifies the frequency of the output pulse in Hz.")]
    public partial class CreateTimestampedPulseFrequencyLed0Payload : CreatePulseFrequencyLed0Payload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the frequency of the output pulse in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PulseFrequencyLed0 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.PulseFrequencyLed0.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the frequency of the output pulse in Hz.
    /// </summary>
    [DisplayName("PulseFrequencyLed1Payload")]
    [Description("Creates a message payload that specifies the frequency of the output pulse in Hz.")]
    public partial class CreatePulseFrequencyLed1Payload
    {
        /// <summary>
        /// Gets or sets the value that specifies the frequency of the output pulse in Hz.
        /// </summary>
        [Range(min: 1, max: long.MaxValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that specifies the frequency of the output pulse in Hz.")]
        public byte PulseFrequencyLed1 { get; set; } = 1;

        /// <summary>
        /// Creates a message payload for the PulseFrequencyLed1 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return PulseFrequencyLed1;
        }

        /// <summary>
        /// Creates a message that specifies the frequency of the output pulse in Hz.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the PulseFrequencyLed1 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.PulseFrequencyLed1.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the frequency of the output pulse in Hz.
    /// </summary>
    [DisplayName("TimestampedPulseFrequencyLed1Payload")]
    [Description("Creates a timestamped message payload that specifies the frequency of the output pulse in Hz.")]
    public partial class CreateTimestampedPulseFrequencyLed1Payload : CreatePulseFrequencyLed1Payload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the frequency of the output pulse in Hz.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the PulseFrequencyLed1 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.PulseFrequencyLed1.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
    /// </summary>
    [DisplayName("RampLed0Payload")]
    [Description("Creates a message payload that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.")]
    public partial class CreateRampLed0Payload
    {
        /// <summary>
        /// Gets or sets the value that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
        /// </summary>
        [Range(min: 1, max: long.MaxValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.")]
        public ushort RampLed0 { get; set; } = 1;

        /// <summary>
        /// Creates a message payload for the RampLed0 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return RampLed0;
        }

        /// <summary>
        /// Creates a message that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the RampLed0 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.RampLed0.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
    /// </summary>
    [DisplayName("TimestampedRampLed0Payload")]
    [Description("Creates a timestamped message payload that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.")]
    public partial class CreateTimestampedRampLed0Payload : CreateRampLed0Payload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the RampLed0 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.RampLed0.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
    /// </summary>
    [DisplayName("RampLed1Payload")]
    [Description("Creates a message payload that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.")]
    public partial class CreateRampLed1Payload
    {
        /// <summary>
        /// Gets or sets the value that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
        /// </summary>
        [Range(min: 1, max: long.MaxValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.")]
        public ushort RampLed1 { get; set; } = 1;

        /// <summary>
        /// Creates a message payload for the RampLed1 register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return RampLed1;
        }

        /// <summary>
        /// Creates a message that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the RampLed1 register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.RampLed1.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
    /// </summary>
    [DisplayName("TimestampedRampLed1Payload")]
    [Description("Creates a timestamped message payload that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.")]
    public partial class CreateTimestampedRampLed1Payload : CreateRampLed1Payload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the ramp time of the transitions between different current/voltage values in milliseconds. The ramp will only work if the pulse function is off.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the RampLed1 register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.RampLed1.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies when the ramps are applied for each DAC/LED.
    /// </summary>
    [DisplayName("RampConfigPayload")]
    [Description("Creates a message payload that specifies when the ramps are applied for each DAC/LED.")]
    public partial class CreateRampConfigPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies when the ramps are applied for each DAC/LED.
        /// </summary>
        [Description("The value that specifies when the ramps are applied for each DAC/LED.")]
        public LedRamps RampConfig { get; set; }

        /// <summary>
        /// Creates a message payload for the RampConfig register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LedRamps GetPayload()
        {
            return RampConfig;
        }

        /// <summary>
        /// Creates a message that specifies when the ramps are applied for each DAC/LED.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the RampConfig register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.RampConfig.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies when the ramps are applied for each DAC/LED.
    /// </summary>
    [DisplayName("TimestampedRampConfigPayload")]
    [Description("Creates a timestamped message payload that specifies when the ramps are applied for each DAC/LED.")]
    public partial class CreateTimestampedRampConfigPayload : CreateRampConfigPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies when the ramps are applied for each DAC/LED.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the RampConfig register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.RampConfig.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the active events in the device.
    /// </summary>
    [DisplayName("EnableEventsPayload")]
    [Description("Creates a message payload that specifies the active events in the device.")]
    public partial class CreateEnableEventsPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the active events in the device.
        /// </summary>
        [Description("The value that specifies the active events in the device.")]
        public CurrentDriverEvents EnableEvents { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableEvents register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public CurrentDriverEvents GetPayload()
        {
            return EnableEvents;
        }

        /// <summary>
        /// Creates a message that specifies the active events in the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableEvents register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.EnableEvents.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the active events in the device.
    /// </summary>
    [DisplayName("TimestampedEnableEventsPayload")]
    [Description("Creates a timestamped message payload that specifies the active events in the device.")]
    public partial class CreateTimestampedEnableEventsPayload : CreateEnableEventsPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the active events in the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableEvents register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.EnableEvents.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Specifies the state of port digital input lines
    /// </summary>
    [Flags]
    public enum DigitalInputs : byte
    {
        None = 0x0,
        DI0 = 0x1,
        DI1 = 0x2
    }

    /// <summary>
    /// Specifies the state of port digital output lines
    /// </summary>
    [Flags]
    public enum DigitalOutputs : byte
    {
        None = 0x0,
        DO0 = 0x1,
        DO1 = 0x2
    }

    /// <summary>
    /// Specifies the state of LED driver's outputs
    /// </summary>
    [Flags]
    public enum LedOutputs : byte
    {
        None = 0x0,
        LED0 = 0x1,
        LED1 = 0x2
    }

    /// <summary>
    /// Specifies the configuration of LED driver's ramps
    /// </summary>
    [Flags]
    public enum LedRamps : byte
    {
        None = 0x0,
        LED0_UP = 0x1,
        LED0_DOWN = 0x2,
        LED1_UP = 0x4,
        LED1_DOWN = 0x8
    }

    /// <summary>
    /// Specifies the active events in the device.
    /// </summary>
    [Flags]
    public enum CurrentDriverEvents : byte
    {
        None = 0x0,
        DIs = 0x1
    }
}
