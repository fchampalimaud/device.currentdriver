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
            { 37, typeof(LedEnable) },
            { 38, typeof(LedDisable) },
            { 39, typeof(LedState) },
            { 40, typeof(LedTargetState) },
            { 41, typeof(Led0Current) },
            { 42, typeof(Led1Current) },
            { 43, typeof(Led0MaxCurrent) },
            { 44, typeof(Led1MaxCurrent) },
            { 45, typeof(Dac0Voltage) },
            { 46, typeof(Dac1Voltage) },
            { 47, typeof(PulseEnable) },
            { 48, typeof(PulseDutyCycleLed0) },
            { 49, typeof(PulseDutyCycleLed1) },
            { 50, typeof(PulseFrequencyLed0) },
            { 51, typeof(PulseFrequencyLed1) },
            { 52, typeof(RampLed0) },
            { 53, typeof(RampLed1) },
            { 54, typeof(RampConfig) },
            { 55, typeof(Protocol0Duration) },
            { 56, typeof(Protocol1Duration) },
            { 57, typeof(Protocol0Delay) },
            { 58, typeof(Protocol1Delay) },
            { 59, typeof(EnableProtocol) },
            { 60, typeof(DisableProtocol) },
            { 61, typeof(DI0Trigger) },
            { 62, typeof(DI1Trigger) },
            { 63, typeof(Reserved0) },
            { 64, typeof(Reserved1) },
            { 65, typeof(Reserved2) },
            { 66, typeof(Reserved3) },
            { 67, typeof(EnableEvents) }
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
    public partial class GetDeviceMetadata : Source<string>
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
    /// Represents an operator that writes the sequence of <see cref="CurrentDriver"/>" messages
    /// to the standard Harp storage format.
    /// </summary>
    [DefaultProperty(nameof(Path))]
    [Description("Writes the sequence of CurrentDriver messages to the standard Harp storage format.")]
    public partial class DeviceDataWriter : Sink<HarpMessage>, INamedElement
    {
        const string BinaryExtension = ".bin";
        const string MetadataFileName = "device.yml";
        readonly Bonsai.Harp.MessageWriter writer = new();

        string INamedElement.Name => nameof(CurrentDriver) + "DataWriter";

        /// <summary>
        /// Gets or sets the relative or absolute path on which to save the message data.
        /// </summary>
        [Description("The relative or absolute path of the directory on which to save the message data.")]
        [Editor("Bonsai.Design.SaveFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string Path
        {
            get => System.IO.Path.GetDirectoryName(writer.FileName);
            set => writer.FileName = System.IO.Path.Combine(value, nameof(CurrentDriver) + BinaryExtension);
        }

        /// <summary>
        /// Gets or sets a value indicating whether element writing should be buffered. If <see langword="true"/>,
        /// the write commands will be queued in memory as fast as possible and will be processed
        /// by the writer in a different thread. Otherwise, writing will be done in the same
        /// thread in which notifications arrive.
        /// </summary>
        [Description("Indicates whether writing should be buffered.")]
        public bool Buffered
        {
            get => writer.Buffered;
            set => writer.Buffered = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to overwrite the output file if it already exists.
        /// </summary>
        [Description("Indicates whether to overwrite the output file if it already exists.")]
        public bool Overwrite
        {
            get => writer.Overwrite;
            set => writer.Overwrite = value;
        }

        /// <summary>
        /// Gets or sets a value specifying how the message filter will use the matching criteria.
        /// </summary>
        [Description("Specifies how the message filter will use the matching criteria.")]
        public FilterType FilterType
        {
            get => writer.FilterType;
            set => writer.FilterType = value;
        }

        /// <summary>
        /// Gets or sets a value specifying the expected message type. If no value is
        /// specified, all messages will be accepted.
        /// </summary>
        [Description("Specifies the expected message type. If no value is specified, all messages will be accepted.")]
        public MessageType? MessageType
        {
            get => writer.MessageType;
            set => writer.MessageType = value;
        }

        private IObservable<TSource> WriteDeviceMetadata<TSource>(IObservable<TSource> source)
        {
            var basePath = Path;
            if (string.IsNullOrEmpty(basePath))
                return source;

            var metadataPath = System.IO.Path.Combine(basePath, MetadataFileName);
            return Observable.Create<TSource>(observer =>
            {
                Bonsai.IO.PathHelper.EnsureDirectory(metadataPath);
                if (System.IO.File.Exists(metadataPath) && !Overwrite)
                {
                    throw new System.IO.IOException(string.Format("The file '{0}' already exists.", metadataPath));
                }

                System.IO.File.WriteAllText(metadataPath, Device.Metadata);
                return source.SubscribeSafe(observer);
            });
        }

        /// <summary>
        /// Writes each Harp message in the sequence to the specified binary file, and the
        /// contents of the device metadata file to a separate text file.
        /// </summary>
        /// <param name="source">The sequence of messages to write to the file.</param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the
        /// messages to a raw binary file, and the contents of the device metadata file
        /// to a separate text file.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            return source.Publish(ps => ps.Merge(
                WriteDeviceMetadata(writer.Process(ps.GroupBy(message => message.Address)))
                .IgnoreElements()
                .Cast<HarpMessage>()));
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated from
        /// the common group register address. The contents of the device metadata file are
        /// written to a separate text file.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique register
        /// address.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the Harp
        /// messages in each group to the corresponding file, and the contents of the device
        /// metadata file to a separate text file.
        /// </returns>
        public IObservable<IGroupedObservable<int, HarpMessage>> Process(IObservable<IGroupedObservable<int, HarpMessage>> source)
        {
            return WriteDeviceMetadata(writer.Process(source));
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated from
        /// the common group register name. The contents of the device metadata file are
        /// written to a separate text file.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique register
        /// type.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the Harp
        /// messages in each group to the corresponding file, and the contents of the device
        /// metadata file to a separate text file.
        /// </returns>
        public IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<IGroupedObservable<Type, HarpMessage>> source)
        {
            return WriteDeviceMetadata(writer.Process(source));
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
    /// <seealso cref="LedEnable"/>
    /// <seealso cref="LedDisable"/>
    /// <seealso cref="LedState"/>
    /// <seealso cref="LedTargetState"/>
    /// <seealso cref="Led0Current"/>
    /// <seealso cref="Led1Current"/>
    /// <seealso cref="Led0MaxCurrent"/>
    /// <seealso cref="Led1MaxCurrent"/>
    /// <seealso cref="Dac0Voltage"/>
    /// <seealso cref="Dac1Voltage"/>
    /// <seealso cref="PulseEnable"/>
    /// <seealso cref="PulseDutyCycleLed0"/>
    /// <seealso cref="PulseDutyCycleLed1"/>
    /// <seealso cref="PulseFrequencyLed0"/>
    /// <seealso cref="PulseFrequencyLed1"/>
    /// <seealso cref="RampLed0"/>
    /// <seealso cref="RampLed1"/>
    /// <seealso cref="RampConfig"/>
    /// <seealso cref="Protocol0Duration"/>
    /// <seealso cref="Protocol1Duration"/>
    /// <seealso cref="Protocol0Delay"/>
    /// <seealso cref="Protocol1Delay"/>
    /// <seealso cref="EnableProtocol"/>
    /// <seealso cref="DisableProtocol"/>
    /// <seealso cref="DI0Trigger"/>
    /// <seealso cref="DI1Trigger"/>
    /// <seealso cref="EnableEvents"/>
    [XmlInclude(typeof(DigitalInputState))]
    [XmlInclude(typeof(OutputSet))]
    [XmlInclude(typeof(OutputClear))]
    [XmlInclude(typeof(OutputToggle))]
    [XmlInclude(typeof(OutputState))]
    [XmlInclude(typeof(LedEnable))]
    [XmlInclude(typeof(LedDisable))]
    [XmlInclude(typeof(LedState))]
    [XmlInclude(typeof(LedTargetState))]
    [XmlInclude(typeof(Led0Current))]
    [XmlInclude(typeof(Led1Current))]
    [XmlInclude(typeof(Led0MaxCurrent))]
    [XmlInclude(typeof(Led1MaxCurrent))]
    [XmlInclude(typeof(Dac0Voltage))]
    [XmlInclude(typeof(Dac1Voltage))]
    [XmlInclude(typeof(PulseEnable))]
    [XmlInclude(typeof(PulseDutyCycleLed0))]
    [XmlInclude(typeof(PulseDutyCycleLed1))]
    [XmlInclude(typeof(PulseFrequencyLed0))]
    [XmlInclude(typeof(PulseFrequencyLed1))]
    [XmlInclude(typeof(RampLed0))]
    [XmlInclude(typeof(RampLed1))]
    [XmlInclude(typeof(RampConfig))]
    [XmlInclude(typeof(Protocol0Duration))]
    [XmlInclude(typeof(Protocol1Duration))]
    [XmlInclude(typeof(Protocol0Delay))]
    [XmlInclude(typeof(Protocol1Delay))]
    [XmlInclude(typeof(EnableProtocol))]
    [XmlInclude(typeof(DisableProtocol))]
    [XmlInclude(typeof(DI0Trigger))]
    [XmlInclude(typeof(DI1Trigger))]
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
    /// <seealso cref="LedEnable"/>
    /// <seealso cref="LedDisable"/>
    /// <seealso cref="LedState"/>
    /// <seealso cref="LedTargetState"/>
    /// <seealso cref="Led0Current"/>
    /// <seealso cref="Led1Current"/>
    /// <seealso cref="Led0MaxCurrent"/>
    /// <seealso cref="Led1MaxCurrent"/>
    /// <seealso cref="Dac0Voltage"/>
    /// <seealso cref="Dac1Voltage"/>
    /// <seealso cref="PulseEnable"/>
    /// <seealso cref="PulseDutyCycleLed0"/>
    /// <seealso cref="PulseDutyCycleLed1"/>
    /// <seealso cref="PulseFrequencyLed0"/>
    /// <seealso cref="PulseFrequencyLed1"/>
    /// <seealso cref="RampLed0"/>
    /// <seealso cref="RampLed1"/>
    /// <seealso cref="RampConfig"/>
    /// <seealso cref="Protocol0Duration"/>
    /// <seealso cref="Protocol1Duration"/>
    /// <seealso cref="Protocol0Delay"/>
    /// <seealso cref="Protocol1Delay"/>
    /// <seealso cref="EnableProtocol"/>
    /// <seealso cref="DisableProtocol"/>
    /// <seealso cref="DI0Trigger"/>
    /// <seealso cref="DI1Trigger"/>
    /// <seealso cref="EnableEvents"/>
    [XmlInclude(typeof(DigitalInputState))]
    [XmlInclude(typeof(OutputSet))]
    [XmlInclude(typeof(OutputClear))]
    [XmlInclude(typeof(OutputToggle))]
    [XmlInclude(typeof(OutputState))]
    [XmlInclude(typeof(LedEnable))]
    [XmlInclude(typeof(LedDisable))]
    [XmlInclude(typeof(LedState))]
    [XmlInclude(typeof(LedTargetState))]
    [XmlInclude(typeof(Led0Current))]
    [XmlInclude(typeof(Led1Current))]
    [XmlInclude(typeof(Led0MaxCurrent))]
    [XmlInclude(typeof(Led1MaxCurrent))]
    [XmlInclude(typeof(Dac0Voltage))]
    [XmlInclude(typeof(Dac1Voltage))]
    [XmlInclude(typeof(PulseEnable))]
    [XmlInclude(typeof(PulseDutyCycleLed0))]
    [XmlInclude(typeof(PulseDutyCycleLed1))]
    [XmlInclude(typeof(PulseFrequencyLed0))]
    [XmlInclude(typeof(PulseFrequencyLed1))]
    [XmlInclude(typeof(RampLed0))]
    [XmlInclude(typeof(RampLed1))]
    [XmlInclude(typeof(RampConfig))]
    [XmlInclude(typeof(Protocol0Duration))]
    [XmlInclude(typeof(Protocol1Duration))]
    [XmlInclude(typeof(Protocol0Delay))]
    [XmlInclude(typeof(Protocol1Delay))]
    [XmlInclude(typeof(EnableProtocol))]
    [XmlInclude(typeof(DisableProtocol))]
    [XmlInclude(typeof(DI0Trigger))]
    [XmlInclude(typeof(DI1Trigger))]
    [XmlInclude(typeof(EnableEvents))]
    [XmlInclude(typeof(TimestampedDigitalInputState))]
    [XmlInclude(typeof(TimestampedOutputSet))]
    [XmlInclude(typeof(TimestampedOutputClear))]
    [XmlInclude(typeof(TimestampedOutputToggle))]
    [XmlInclude(typeof(TimestampedOutputState))]
    [XmlInclude(typeof(TimestampedLedEnable))]
    [XmlInclude(typeof(TimestampedLedDisable))]
    [XmlInclude(typeof(TimestampedLedState))]
    [XmlInclude(typeof(TimestampedLedTargetState))]
    [XmlInclude(typeof(TimestampedLed0Current))]
    [XmlInclude(typeof(TimestampedLed1Current))]
    [XmlInclude(typeof(TimestampedLed0MaxCurrent))]
    [XmlInclude(typeof(TimestampedLed1MaxCurrent))]
    [XmlInclude(typeof(TimestampedDac0Voltage))]
    [XmlInclude(typeof(TimestampedDac1Voltage))]
    [XmlInclude(typeof(TimestampedPulseEnable))]
    [XmlInclude(typeof(TimestampedPulseDutyCycleLed0))]
    [XmlInclude(typeof(TimestampedPulseDutyCycleLed1))]
    [XmlInclude(typeof(TimestampedPulseFrequencyLed0))]
    [XmlInclude(typeof(TimestampedPulseFrequencyLed1))]
    [XmlInclude(typeof(TimestampedRampLed0))]
    [XmlInclude(typeof(TimestampedRampLed1))]
    [XmlInclude(typeof(TimestampedRampConfig))]
    [XmlInclude(typeof(TimestampedProtocol0Duration))]
    [XmlInclude(typeof(TimestampedProtocol1Duration))]
    [XmlInclude(typeof(TimestampedProtocol0Delay))]
    [XmlInclude(typeof(TimestampedProtocol1Delay))]
    [XmlInclude(typeof(TimestampedEnableProtocol))]
    [XmlInclude(typeof(TimestampedDisableProtocol))]
    [XmlInclude(typeof(TimestampedDI0Trigger))]
    [XmlInclude(typeof(TimestampedDI1Trigger))]
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
    /// <seealso cref="LedEnable"/>
    /// <seealso cref="LedDisable"/>
    /// <seealso cref="LedState"/>
    /// <seealso cref="LedTargetState"/>
    /// <seealso cref="Led0Current"/>
    /// <seealso cref="Led1Current"/>
    /// <seealso cref="Led0MaxCurrent"/>
    /// <seealso cref="Led1MaxCurrent"/>
    /// <seealso cref="Dac0Voltage"/>
    /// <seealso cref="Dac1Voltage"/>
    /// <seealso cref="PulseEnable"/>
    /// <seealso cref="PulseDutyCycleLed0"/>
    /// <seealso cref="PulseDutyCycleLed1"/>
    /// <seealso cref="PulseFrequencyLed0"/>
    /// <seealso cref="PulseFrequencyLed1"/>
    /// <seealso cref="RampLed0"/>
    /// <seealso cref="RampLed1"/>
    /// <seealso cref="RampConfig"/>
    /// <seealso cref="Protocol0Duration"/>
    /// <seealso cref="Protocol1Duration"/>
    /// <seealso cref="Protocol0Delay"/>
    /// <seealso cref="Protocol1Delay"/>
    /// <seealso cref="EnableProtocol"/>
    /// <seealso cref="DisableProtocol"/>
    /// <seealso cref="DI0Trigger"/>
    /// <seealso cref="DI1Trigger"/>
    /// <seealso cref="EnableEvents"/>
    [XmlInclude(typeof(DigitalInputState))]
    [XmlInclude(typeof(OutputSet))]
    [XmlInclude(typeof(OutputClear))]
    [XmlInclude(typeof(OutputToggle))]
    [XmlInclude(typeof(OutputState))]
    [XmlInclude(typeof(LedEnable))]
    [XmlInclude(typeof(LedDisable))]
    [XmlInclude(typeof(LedState))]
    [XmlInclude(typeof(LedTargetState))]
    [XmlInclude(typeof(Led0Current))]
    [XmlInclude(typeof(Led1Current))]
    [XmlInclude(typeof(Led0MaxCurrent))]
    [XmlInclude(typeof(Led1MaxCurrent))]
    [XmlInclude(typeof(Dac0Voltage))]
    [XmlInclude(typeof(Dac1Voltage))]
    [XmlInclude(typeof(PulseEnable))]
    [XmlInclude(typeof(PulseDutyCycleLed0))]
    [XmlInclude(typeof(PulseDutyCycleLed1))]
    [XmlInclude(typeof(PulseFrequencyLed0))]
    [XmlInclude(typeof(PulseFrequencyLed1))]
    [XmlInclude(typeof(RampLed0))]
    [XmlInclude(typeof(RampLed1))]
    [XmlInclude(typeof(RampConfig))]
    [XmlInclude(typeof(Protocol0Duration))]
    [XmlInclude(typeof(Protocol1Duration))]
    [XmlInclude(typeof(Protocol0Delay))]
    [XmlInclude(typeof(Protocol1Delay))]
    [XmlInclude(typeof(EnableProtocol))]
    [XmlInclude(typeof(DisableProtocol))]
    [XmlInclude(typeof(DI0Trigger))]
    [XmlInclude(typeof(DI1Trigger))]
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
        public const PayloadType RegisterType = PayloadType.U8;

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
            return (DigitalOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputSet"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
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
            return HarpMessage.FromByte(Address, messageType, (byte)value);
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
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
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
        public const PayloadType RegisterType = PayloadType.U8;

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
            return (DigitalOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputClear"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
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
            return HarpMessage.FromByte(Address, messageType, (byte)value);
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
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
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
        public const PayloadType RegisterType = PayloadType.U8;

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
            return (DigitalOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputToggle"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
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
            return HarpMessage.FromByte(Address, messageType, (byte)value);
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
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
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
        public const PayloadType RegisterType = PayloadType.U8;

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
            return (DigitalOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OutputState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DigitalOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
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
            return HarpMessage.FromByte(Address, messageType, (byte)value);
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
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
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
    /// Represents a register that enable driver on the selected output.
    /// </summary>
    [Description("Enable driver on the selected output")]
    public partial class LedEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="LedEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = 37;

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
        public const int Address = 38;

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
    /// Represents a register that control the respective LED output.
    /// </summary>
    [Description("Control the respective LED output")]
    public partial class LedState
    {
        /// <summary>
        /// Represents the address of the <see cref="LedState"/> register. This field is constant.
        /// </summary>
        public const int Address = 39;

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
    /// Represents a register that sends an event when the LED reaches the target value.
    /// </summary>
    [Description("Sends an event when the LED reaches the target value")]
    public partial class LedTargetState
    {
        /// <summary>
        /// Represents the address of the <see cref="LedTargetState"/> register. This field is constant.
        /// </summary>
        public const int Address = 40;

        /// <summary>
        /// Represents the payload type of the <see cref="LedTargetState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="LedTargetState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="LedTargetState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LedOutputs GetPayload(HarpMessage message)
        {
            return (LedOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="LedTargetState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((LedOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="LedTargetState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LedTargetState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="LedTargetState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="LedTargetState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// LedTargetState register.
    /// </summary>
    /// <seealso cref="LedTargetState"/>
    [Description("Filters and selects timestamped messages from the LedTargetState register.")]
    public partial class TimestampedLedTargetState
    {
        /// <summary>
        /// Represents the address of the <see cref="LedTargetState"/> register. This field is constant.
        /// </summary>
        public const int Address = LedTargetState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="LedTargetState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetPayload(HarpMessage message)
        {
            return LedTargetState.GetTimestampedPayload(message);
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
        public const int Address = 41;

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
        public const int Address = 42;

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
    /// Represents a register that configuration of current to drive LED 0 [0:1000] mA.
    /// </summary>
    [Description("Configuration of current to drive LED 0 [0:1000] mA")]
    public partial class Led0MaxCurrent
    {
        /// <summary>
        /// Represents the address of the <see cref="Led0MaxCurrent"/> register. This field is constant.
        /// </summary>
        public const int Address = 43;

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
        public const int Address = 44;

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
    /// Represents a register that configuration of DAC 0 voltage [0:5000] mV.
    /// </summary>
    [Description("Configuration of DAC 0 voltage [0:5000] mV")]
    public partial class Dac0Voltage
    {
        /// <summary>
        /// Represents the address of the <see cref="Dac0Voltage"/> register. This field is constant.
        /// </summary>
        public const int Address = 45;

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
        public const int Address = 46;

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
    /// Represents a register that enables the pulse function for the specified output DACs/LEDs.
    /// </summary>
    [Description("Enables the pulse function for the specified output DACs/LEDs")]
    public partial class PulseEnable
    {
        /// <summary>
        /// Represents the address of the <see cref="PulseEnable"/> register. This field is constant.
        /// </summary>
        public const int Address = 47;

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
        public const int Address = 48;

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
        public const int Address = 49;

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
        public const int Address = 50;

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
        public const int Address = 51;

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
        public const int Address = 52;

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
        public const int Address = 53;

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
        public const int Address = 54;

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
    /// Represents a register that specifies the duration of LED0 protocol.
    /// </summary>
    [Description("Specifies the duration of LED0 protocol")]
    public partial class Protocol0Duration
    {
        /// <summary>
        /// Represents the address of the <see cref="Protocol0Duration"/> register. This field is constant.
        /// </summary>
        public const int Address = 55;

        /// <summary>
        /// Represents the payload type of the <see cref="Protocol0Duration"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Protocol0Duration"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Protocol0Duration"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Protocol0Duration"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Protocol0Duration"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Protocol0Duration"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Protocol0Duration"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Protocol0Duration"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Protocol0Duration register.
    /// </summary>
    /// <seealso cref="Protocol0Duration"/>
    [Description("Filters and selects timestamped messages from the Protocol0Duration register.")]
    public partial class TimestampedProtocol0Duration
    {
        /// <summary>
        /// Represents the address of the <see cref="Protocol0Duration"/> register. This field is constant.
        /// </summary>
        public const int Address = Protocol0Duration.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Protocol0Duration"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Protocol0Duration.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the duration of LED1 protocol.
    /// </summary>
    [Description("Specifies the duration of LED1 protocol")]
    public partial class Protocol1Duration
    {
        /// <summary>
        /// Represents the address of the <see cref="Protocol1Duration"/> register. This field is constant.
        /// </summary>
        public const int Address = 56;

        /// <summary>
        /// Represents the payload type of the <see cref="Protocol1Duration"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Protocol1Duration"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Protocol1Duration"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Protocol1Duration"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Protocol1Duration"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Protocol1Duration"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Protocol1Duration"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Protocol1Duration"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Protocol1Duration register.
    /// </summary>
    /// <seealso cref="Protocol1Duration"/>
    [Description("Filters and selects timestamped messages from the Protocol1Duration register.")]
    public partial class TimestampedProtocol1Duration
    {
        /// <summary>
        /// Represents the address of the <see cref="Protocol1Duration"/> register. This field is constant.
        /// </summary>
        public const int Address = Protocol1Duration.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Protocol1Duration"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Protocol1Duration.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the delay of the LED0 protocol.
    /// </summary>
    [Description("Specifies the delay of the LED0 protocol")]
    public partial class Protocol0Delay
    {
        /// <summary>
        /// Represents the address of the <see cref="Protocol0Delay"/> register. This field is constant.
        /// </summary>
        public const int Address = 57;

        /// <summary>
        /// Represents the payload type of the <see cref="Protocol0Delay"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Protocol0Delay"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Protocol0Delay"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Protocol0Delay"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Protocol0Delay"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Protocol0Delay"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Protocol0Delay"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Protocol0Delay"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Protocol0Delay register.
    /// </summary>
    /// <seealso cref="Protocol0Delay"/>
    [Description("Filters and selects timestamped messages from the Protocol0Delay register.")]
    public partial class TimestampedProtocol0Delay
    {
        /// <summary>
        /// Represents the address of the <see cref="Protocol0Delay"/> register. This field is constant.
        /// </summary>
        public const int Address = Protocol0Delay.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Protocol0Delay"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Protocol0Delay.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the delay of the LED1 protocol.
    /// </summary>
    [Description("Specifies the delay of the LED1 protocol")]
    public partial class Protocol1Delay
    {
        /// <summary>
        /// Represents the address of the <see cref="Protocol1Delay"/> register. This field is constant.
        /// </summary>
        public const int Address = 58;

        /// <summary>
        /// Represents the payload type of the <see cref="Protocol1Delay"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Protocol1Delay"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Protocol1Delay"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Protocol1Delay"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Protocol1Delay"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Protocol1Delay"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Protocol1Delay"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Protocol1Delay"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Protocol1Delay register.
    /// </summary>
    /// <seealso cref="Protocol1Delay"/>
    [Description("Filters and selects timestamped messages from the Protocol1Delay register.")]
    public partial class TimestampedProtocol1Delay
    {
        /// <summary>
        /// Represents the address of the <see cref="Protocol1Delay"/> register. This field is constant.
        /// </summary>
        public const int Address = Protocol1Delay.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Protocol1Delay"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Protocol1Delay.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enable the respective protocol.
    /// </summary>
    [Description("Enable the respective protocol")]
    public partial class EnableProtocol
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableProtocol"/> register. This field is constant.
        /// </summary>
        public const int Address = 59;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableProtocol"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableProtocol"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableProtocol"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LedOutputs GetPayload(HarpMessage message)
        {
            return (LedOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableProtocol"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((LedOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableProtocol"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableProtocol"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableProtocol"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableProtocol"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableProtocol register.
    /// </summary>
    /// <seealso cref="EnableProtocol"/>
    [Description("Filters and selects timestamped messages from the EnableProtocol register.")]
    public partial class TimestampedEnableProtocol
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableProtocol"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableProtocol.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableProtocol"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetPayload(HarpMessage message)
        {
            return EnableProtocol.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that disable the respective protocol.
    /// </summary>
    [Description("Disable the respective protocol")]
    public partial class DisableProtocol
    {
        /// <summary>
        /// Represents the address of the <see cref="DisableProtocol"/> register. This field is constant.
        /// </summary>
        public const int Address = 60;

        /// <summary>
        /// Represents the payload type of the <see cref="DisableProtocol"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="DisableProtocol"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="DisableProtocol"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static LedOutputs GetPayload(HarpMessage message)
        {
            return (LedOutputs)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="DisableProtocol"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((LedOutputs)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="DisableProtocol"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DisableProtocol"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="DisableProtocol"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DisableProtocol"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, LedOutputs value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// DisableProtocol register.
    /// </summary>
    /// <seealso cref="DisableProtocol"/>
    [Description("Filters and selects timestamped messages from the DisableProtocol register.")]
    public partial class TimestampedDisableProtocol
    {
        /// <summary>
        /// Represents the address of the <see cref="DisableProtocol"/> register. This field is constant.
        /// </summary>
        public const int Address = DisableProtocol.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="DisableProtocol"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<LedOutputs> GetPayload(HarpMessage message)
        {
            return DisableProtocol.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configures the callback function triggered when DI0 is triggered.
    /// </summary>
    [Description("Configures the callback function triggered when DI0 is triggered")]
    public partial class DI0Trigger
    {
        /// <summary>
        /// Represents the address of the <see cref="DI0Trigger"/> register. This field is constant.
        /// </summary>
        public const int Address = 61;

        /// <summary>
        /// Represents the payload type of the <see cref="DI0Trigger"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="DI0Trigger"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="DI0Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DITriggerConfig GetPayload(HarpMessage message)
        {
            return (DITriggerConfig)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="DI0Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DITriggerConfig> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((DITriggerConfig)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="DI0Trigger"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DI0Trigger"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DITriggerConfig value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="DI0Trigger"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DI0Trigger"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DITriggerConfig value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// DI0Trigger register.
    /// </summary>
    /// <seealso cref="DI0Trigger"/>
    [Description("Filters and selects timestamped messages from the DI0Trigger register.")]
    public partial class TimestampedDI0Trigger
    {
        /// <summary>
        /// Represents the address of the <see cref="DI0Trigger"/> register. This field is constant.
        /// </summary>
        public const int Address = DI0Trigger.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="DI0Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DITriggerConfig> GetPayload(HarpMessage message)
        {
            return DI0Trigger.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that configures the callback function triggered when DI1 is triggered.
    /// </summary>
    [Description("Configures the callback function triggered when DI1 is triggered")]
    public partial class DI1Trigger
    {
        /// <summary>
        /// Represents the address of the <see cref="DI1Trigger"/> register. This field is constant.
        /// </summary>
        public const int Address = 62;

        /// <summary>
        /// Represents the payload type of the <see cref="DI1Trigger"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="DI1Trigger"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="DI1Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static DITriggerConfig GetPayload(HarpMessage message)
        {
            return (DITriggerConfig)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="DI1Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DITriggerConfig> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((DITriggerConfig)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="DI1Trigger"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DI1Trigger"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, DITriggerConfig value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="DI1Trigger"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DI1Trigger"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, DITriggerConfig value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// DI1Trigger register.
    /// </summary>
    /// <seealso cref="DI1Trigger"/>
    [Description("Filters and selects timestamped messages from the DI1Trigger register.")]
    public partial class TimestampedDI1Trigger
    {
        /// <summary>
        /// Represents the address of the <see cref="DI1Trigger"/> register. This field is constant.
        /// </summary>
        public const int Address = DI1Trigger.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="DI1Trigger"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<DITriggerConfig> GetPayload(HarpMessage message)
        {
            return DI1Trigger.GetTimestampedPayload(message);
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
        public const int Address = 63;

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
        public const int Address = 64;

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
        public const int Address = 65;

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
        public const int Address = 66;

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
        public const int Address = 67;

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
    /// <seealso cref="CreateLedEnablePayload"/>
    /// <seealso cref="CreateLedDisablePayload"/>
    /// <seealso cref="CreateLedStatePayload"/>
    /// <seealso cref="CreateLedTargetStatePayload"/>
    /// <seealso cref="CreateLed0CurrentPayload"/>
    /// <seealso cref="CreateLed1CurrentPayload"/>
    /// <seealso cref="CreateLed0MaxCurrentPayload"/>
    /// <seealso cref="CreateLed1MaxCurrentPayload"/>
    /// <seealso cref="CreateDac0VoltagePayload"/>
    /// <seealso cref="CreateDac1VoltagePayload"/>
    /// <seealso cref="CreatePulseEnablePayload"/>
    /// <seealso cref="CreatePulseDutyCycleLed0Payload"/>
    /// <seealso cref="CreatePulseDutyCycleLed1Payload"/>
    /// <seealso cref="CreatePulseFrequencyLed0Payload"/>
    /// <seealso cref="CreatePulseFrequencyLed1Payload"/>
    /// <seealso cref="CreateRampLed0Payload"/>
    /// <seealso cref="CreateRampLed1Payload"/>
    /// <seealso cref="CreateRampConfigPayload"/>
    /// <seealso cref="CreateProtocol0DurationPayload"/>
    /// <seealso cref="CreateProtocol1DurationPayload"/>
    /// <seealso cref="CreateProtocol0DelayPayload"/>
    /// <seealso cref="CreateProtocol1DelayPayload"/>
    /// <seealso cref="CreateEnableProtocolPayload"/>
    /// <seealso cref="CreateDisableProtocolPayload"/>
    /// <seealso cref="CreateDI0TriggerPayload"/>
    /// <seealso cref="CreateDI1TriggerPayload"/>
    /// <seealso cref="CreateEnableEventsPayload"/>
    [XmlInclude(typeof(CreateDigitalInputStatePayload))]
    [XmlInclude(typeof(CreateOutputSetPayload))]
    [XmlInclude(typeof(CreateOutputClearPayload))]
    [XmlInclude(typeof(CreateOutputTogglePayload))]
    [XmlInclude(typeof(CreateOutputStatePayload))]
    [XmlInclude(typeof(CreateLedEnablePayload))]
    [XmlInclude(typeof(CreateLedDisablePayload))]
    [XmlInclude(typeof(CreateLedStatePayload))]
    [XmlInclude(typeof(CreateLedTargetStatePayload))]
    [XmlInclude(typeof(CreateLed0CurrentPayload))]
    [XmlInclude(typeof(CreateLed1CurrentPayload))]
    [XmlInclude(typeof(CreateLed0MaxCurrentPayload))]
    [XmlInclude(typeof(CreateLed1MaxCurrentPayload))]
    [XmlInclude(typeof(CreateDac0VoltagePayload))]
    [XmlInclude(typeof(CreateDac1VoltagePayload))]
    [XmlInclude(typeof(CreatePulseEnablePayload))]
    [XmlInclude(typeof(CreatePulseDutyCycleLed0Payload))]
    [XmlInclude(typeof(CreatePulseDutyCycleLed1Payload))]
    [XmlInclude(typeof(CreatePulseFrequencyLed0Payload))]
    [XmlInclude(typeof(CreatePulseFrequencyLed1Payload))]
    [XmlInclude(typeof(CreateRampLed0Payload))]
    [XmlInclude(typeof(CreateRampLed1Payload))]
    [XmlInclude(typeof(CreateRampConfigPayload))]
    [XmlInclude(typeof(CreateProtocol0DurationPayload))]
    [XmlInclude(typeof(CreateProtocol1DurationPayload))]
    [XmlInclude(typeof(CreateProtocol0DelayPayload))]
    [XmlInclude(typeof(CreateProtocol1DelayPayload))]
    [XmlInclude(typeof(CreateEnableProtocolPayload))]
    [XmlInclude(typeof(CreateDisableProtocolPayload))]
    [XmlInclude(typeof(CreateDI0TriggerPayload))]
    [XmlInclude(typeof(CreateDI1TriggerPayload))]
    [XmlInclude(typeof(CreateEnableEventsPayload))]
    [XmlInclude(typeof(CreateTimestampedDigitalInputStatePayload))]
    [XmlInclude(typeof(CreateTimestampedOutputSetPayload))]
    [XmlInclude(typeof(CreateTimestampedOutputClearPayload))]
    [XmlInclude(typeof(CreateTimestampedOutputTogglePayload))]
    [XmlInclude(typeof(CreateTimestampedOutputStatePayload))]
    [XmlInclude(typeof(CreateTimestampedLedEnablePayload))]
    [XmlInclude(typeof(CreateTimestampedLedDisablePayload))]
    [XmlInclude(typeof(CreateTimestampedLedStatePayload))]
    [XmlInclude(typeof(CreateTimestampedLedTargetStatePayload))]
    [XmlInclude(typeof(CreateTimestampedLed0CurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedLed1CurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedLed0MaxCurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedLed1MaxCurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedDac0VoltagePayload))]
    [XmlInclude(typeof(CreateTimestampedDac1VoltagePayload))]
    [XmlInclude(typeof(CreateTimestampedPulseEnablePayload))]
    [XmlInclude(typeof(CreateTimestampedPulseDutyCycleLed0Payload))]
    [XmlInclude(typeof(CreateTimestampedPulseDutyCycleLed1Payload))]
    [XmlInclude(typeof(CreateTimestampedPulseFrequencyLed0Payload))]
    [XmlInclude(typeof(CreateTimestampedPulseFrequencyLed1Payload))]
    [XmlInclude(typeof(CreateTimestampedRampLed0Payload))]
    [XmlInclude(typeof(CreateTimestampedRampLed1Payload))]
    [XmlInclude(typeof(CreateTimestampedRampConfigPayload))]
    [XmlInclude(typeof(CreateTimestampedProtocol0DurationPayload))]
    [XmlInclude(typeof(CreateTimestampedProtocol1DurationPayload))]
    [XmlInclude(typeof(CreateTimestampedProtocol0DelayPayload))]
    [XmlInclude(typeof(CreateTimestampedProtocol1DelayPayload))]
    [XmlInclude(typeof(CreateTimestampedEnableProtocolPayload))]
    [XmlInclude(typeof(CreateTimestampedDisableProtocolPayload))]
    [XmlInclude(typeof(CreateTimestampedDI0TriggerPayload))]
    [XmlInclude(typeof(CreateTimestampedDI1TriggerPayload))]
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
    /// that control the respective LED output.
    /// </summary>
    [DisplayName("LedStatePayload")]
    [Description("Creates a message payload that control the respective LED output.")]
    public partial class CreateLedStatePayload
    {
        /// <summary>
        /// Gets or sets the value that control the respective LED output.
        /// </summary>
        [Description("The value that control the respective LED output.")]
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
        /// Creates a message that control the respective LED output.
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
    /// that control the respective LED output.
    /// </summary>
    [DisplayName("TimestampedLedStatePayload")]
    [Description("Creates a timestamped message payload that control the respective LED output.")]
    public partial class CreateTimestampedLedStatePayload : CreateLedStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that control the respective LED output.
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
    /// that sends an event when the LED reaches the target value.
    /// </summary>
    [DisplayName("LedTargetStatePayload")]
    [Description("Creates a message payload that sends an event when the LED reaches the target value.")]
    public partial class CreateLedTargetStatePayload
    {
        /// <summary>
        /// Gets or sets the value that sends an event when the LED reaches the target value.
        /// </summary>
        [Description("The value that sends an event when the LED reaches the target value.")]
        public LedOutputs LedTargetState { get; set; }

        /// <summary>
        /// Creates a message payload for the LedTargetState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LedOutputs GetPayload()
        {
            return LedTargetState;
        }

        /// <summary>
        /// Creates a message that sends an event when the LED reaches the target value.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the LedTargetState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.LedTargetState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that sends an event when the LED reaches the target value.
    /// </summary>
    [DisplayName("TimestampedLedTargetStatePayload")]
    [Description("Creates a timestamped message payload that sends an event when the LED reaches the target value.")]
    public partial class CreateTimestampedLedTargetStatePayload : CreateLedTargetStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that sends an event when the LED reaches the target value.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the LedTargetState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.LedTargetState.FromPayload(timestamp, messageType, GetPayload());
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
    /// that specifies the duration of LED0 protocol.
    /// </summary>
    [DisplayName("Protocol0DurationPayload")]
    [Description("Creates a message payload that specifies the duration of LED0 protocol.")]
    public partial class CreateProtocol0DurationPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the duration of LED0 protocol.
        /// </summary>
        [Description("The value that specifies the duration of LED0 protocol.")]
        public ushort Protocol0Duration { get; set; }

        /// <summary>
        /// Creates a message payload for the Protocol0Duration register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Protocol0Duration;
        }

        /// <summary>
        /// Creates a message that specifies the duration of LED0 protocol.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Protocol0Duration register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Protocol0Duration.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the duration of LED0 protocol.
    /// </summary>
    [DisplayName("TimestampedProtocol0DurationPayload")]
    [Description("Creates a timestamped message payload that specifies the duration of LED0 protocol.")]
    public partial class CreateTimestampedProtocol0DurationPayload : CreateProtocol0DurationPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the duration of LED0 protocol.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Protocol0Duration register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Protocol0Duration.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the duration of LED1 protocol.
    /// </summary>
    [DisplayName("Protocol1DurationPayload")]
    [Description("Creates a message payload that specifies the duration of LED1 protocol.")]
    public partial class CreateProtocol1DurationPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the duration of LED1 protocol.
        /// </summary>
        [Description("The value that specifies the duration of LED1 protocol.")]
        public ushort Protocol1Duration { get; set; }

        /// <summary>
        /// Creates a message payload for the Protocol1Duration register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Protocol1Duration;
        }

        /// <summary>
        /// Creates a message that specifies the duration of LED1 protocol.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Protocol1Duration register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Protocol1Duration.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the duration of LED1 protocol.
    /// </summary>
    [DisplayName("TimestampedProtocol1DurationPayload")]
    [Description("Creates a timestamped message payload that specifies the duration of LED1 protocol.")]
    public partial class CreateTimestampedProtocol1DurationPayload : CreateProtocol1DurationPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the duration of LED1 protocol.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Protocol1Duration register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Protocol1Duration.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the delay of the LED0 protocol.
    /// </summary>
    [DisplayName("Protocol0DelayPayload")]
    [Description("Creates a message payload that specifies the delay of the LED0 protocol.")]
    public partial class CreateProtocol0DelayPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the delay of the LED0 protocol.
        /// </summary>
        [Description("The value that specifies the delay of the LED0 protocol.")]
        public ushort Protocol0Delay { get; set; }

        /// <summary>
        /// Creates a message payload for the Protocol0Delay register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Protocol0Delay;
        }

        /// <summary>
        /// Creates a message that specifies the delay of the LED0 protocol.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Protocol0Delay register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Protocol0Delay.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the delay of the LED0 protocol.
    /// </summary>
    [DisplayName("TimestampedProtocol0DelayPayload")]
    [Description("Creates a timestamped message payload that specifies the delay of the LED0 protocol.")]
    public partial class CreateTimestampedProtocol0DelayPayload : CreateProtocol0DelayPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the delay of the LED0 protocol.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Protocol0Delay register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Protocol0Delay.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the delay of the LED1 protocol.
    /// </summary>
    [DisplayName("Protocol1DelayPayload")]
    [Description("Creates a message payload that specifies the delay of the LED1 protocol.")]
    public partial class CreateProtocol1DelayPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the delay of the LED1 protocol.
        /// </summary>
        [Description("The value that specifies the delay of the LED1 protocol.")]
        public ushort Protocol1Delay { get; set; }

        /// <summary>
        /// Creates a message payload for the Protocol1Delay register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Protocol1Delay;
        }

        /// <summary>
        /// Creates a message that specifies the delay of the LED1 protocol.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Protocol1Delay register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.Protocol1Delay.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the delay of the LED1 protocol.
    /// </summary>
    [DisplayName("TimestampedProtocol1DelayPayload")]
    [Description("Creates a timestamped message payload that specifies the delay of the LED1 protocol.")]
    public partial class CreateTimestampedProtocol1DelayPayload : CreateProtocol1DelayPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the delay of the LED1 protocol.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Protocol1Delay register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.Protocol1Delay.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enable the respective protocol.
    /// </summary>
    [DisplayName("EnableProtocolPayload")]
    [Description("Creates a message payload that enable the respective protocol.")]
    public partial class CreateEnableProtocolPayload
    {
        /// <summary>
        /// Gets or sets the value that enable the respective protocol.
        /// </summary>
        [Description("The value that enable the respective protocol.")]
        public LedOutputs EnableProtocol { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableProtocol register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LedOutputs GetPayload()
        {
            return EnableProtocol;
        }

        /// <summary>
        /// Creates a message that enable the respective protocol.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableProtocol register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.EnableProtocol.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enable the respective protocol.
    /// </summary>
    [DisplayName("TimestampedEnableProtocolPayload")]
    [Description("Creates a timestamped message payload that enable the respective protocol.")]
    public partial class CreateTimestampedEnableProtocolPayload : CreateEnableProtocolPayload
    {
        /// <summary>
        /// Creates a timestamped message that enable the respective protocol.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableProtocol register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.EnableProtocol.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that disable the respective protocol.
    /// </summary>
    [DisplayName("DisableProtocolPayload")]
    [Description("Creates a message payload that disable the respective protocol.")]
    public partial class CreateDisableProtocolPayload
    {
        /// <summary>
        /// Gets or sets the value that disable the respective protocol.
        /// </summary>
        [Description("The value that disable the respective protocol.")]
        public LedOutputs DisableProtocol { get; set; }

        /// <summary>
        /// Creates a message payload for the DisableProtocol register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public LedOutputs GetPayload()
        {
            return DisableProtocol;
        }

        /// <summary>
        /// Creates a message that disable the respective protocol.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the DisableProtocol register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.DisableProtocol.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that disable the respective protocol.
    /// </summary>
    [DisplayName("TimestampedDisableProtocolPayload")]
    [Description("Creates a timestamped message payload that disable the respective protocol.")]
    public partial class CreateTimestampedDisableProtocolPayload : CreateDisableProtocolPayload
    {
        /// <summary>
        /// Creates a timestamped message that disable the respective protocol.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the DisableProtocol register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.DisableProtocol.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configures the callback function triggered when DI0 is triggered.
    /// </summary>
    [DisplayName("DI0TriggerPayload")]
    [Description("Creates a message payload that configures the callback function triggered when DI0 is triggered.")]
    public partial class CreateDI0TriggerPayload
    {
        /// <summary>
        /// Gets or sets the value that configures the callback function triggered when DI0 is triggered.
        /// </summary>
        [Description("The value that configures the callback function triggered when DI0 is triggered.")]
        public DITriggerConfig DI0Trigger { get; set; }

        /// <summary>
        /// Creates a message payload for the DI0Trigger register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DITriggerConfig GetPayload()
        {
            return DI0Trigger;
        }

        /// <summary>
        /// Creates a message that configures the callback function triggered when DI0 is triggered.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the DI0Trigger register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.DI0Trigger.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configures the callback function triggered when DI0 is triggered.
    /// </summary>
    [DisplayName("TimestampedDI0TriggerPayload")]
    [Description("Creates a timestamped message payload that configures the callback function triggered when DI0 is triggered.")]
    public partial class CreateTimestampedDI0TriggerPayload : CreateDI0TriggerPayload
    {
        /// <summary>
        /// Creates a timestamped message that configures the callback function triggered when DI0 is triggered.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the DI0Trigger register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.DI0Trigger.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that configures the callback function triggered when DI1 is triggered.
    /// </summary>
    [DisplayName("DI1TriggerPayload")]
    [Description("Creates a message payload that configures the callback function triggered when DI1 is triggered.")]
    public partial class CreateDI1TriggerPayload
    {
        /// <summary>
        /// Gets or sets the value that configures the callback function triggered when DI1 is triggered.
        /// </summary>
        [Description("The value that configures the callback function triggered when DI1 is triggered.")]
        public DITriggerConfig DI1Trigger { get; set; }

        /// <summary>
        /// Creates a message payload for the DI1Trigger register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public DITriggerConfig GetPayload()
        {
            return DI1Trigger;
        }

        /// <summary>
        /// Creates a message that configures the callback function triggered when DI1 is triggered.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the DI1Trigger register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Harp.CurrentDriver.DI1Trigger.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that configures the callback function triggered when DI1 is triggered.
    /// </summary>
    [DisplayName("TimestampedDI1TriggerPayload")]
    [Description("Creates a timestamped message payload that configures the callback function triggered when DI1 is triggered.")]
    public partial class CreateTimestampedDI1TriggerPayload : CreateDI1TriggerPayload
    {
        /// <summary>
        /// Creates a timestamped message that configures the callback function triggered when DI1 is triggered.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the DI1Trigger register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Harp.CurrentDriver.DI1Trigger.FromPayload(timestamp, messageType, GetPayload());
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
        Led0 = 0x1,
        Led1 = 0x2
    }

    /// <summary>
    /// Specifies the configuration of LED driver's ramps
    /// </summary>
    [Flags]
    public enum LedRamps : byte
    {
        None = 0x0,
        Led0Rise = 0x1,
        Led0Fall = 0x2,
        Led1Rise = 0x4,
        Led1Fall = 0x8
    }

    /// <summary>
    /// Specifies the active events in the device
    /// </summary>
    [Flags]
    public enum CurrentDriverEvents : byte
    {
        None = 0x0,
        DIs = 0x1,
        LedState = 0x2
    }

    /// <summary>
    /// Specifies the way digital inputs work
    /// </summary>
    public enum DITriggerConfig : byte
    {
        Digital = 0,
        ControlLed = 1,
        StartProtocol = 2,
        StartAndStopProtocol = 3
    }

    internal static partial class PayloadMarshal
    {
        internal static T[] GetSubArray<T>(T[] array, int offset, int count)
        {
            var result = new T[count];
            Array.Copy(array, offset, result, 0, count);
            return result;
        }

        internal static byte ReadByte(ArraySegment<byte> segment) => segment.Array[segment.Offset];

        internal static sbyte ReadSByte(ArraySegment<byte> segment) => (sbyte)segment.Array[segment.Offset];

        internal static ushort ReadUInt16(ArraySegment<byte> segment) => BitConverter.ToUInt16(segment.Array, segment.Offset);

        internal static short ReadInt16(ArraySegment<byte> segment) => BitConverter.ToInt16(segment.Array, segment.Offset);

        internal static uint ReadUInt32(ArraySegment<byte> segment) => BitConverter.ToUInt32(segment.Array, segment.Offset);

        internal static int ReadInt32(ArraySegment<byte> segment) => BitConverter.ToInt32(segment.Array, segment.Offset);

        internal static ulong ReadUInt64(ArraySegment<byte> segment) => BitConverter.ToUInt64(segment.Array, segment.Offset);

        internal static long ReadInt64(ArraySegment<byte> segment) => BitConverter.ToInt64(segment.Array, segment.Offset);

        internal static float ReadSingle(ArraySegment<byte> segment) => BitConverter.ToSingle(segment.Array, segment.Offset);

        internal static string ReadUtf8String(ArraySegment<byte> segment)
        {
            var count = Array.IndexOf(segment.Array, (byte)0, segment.Offset, segment.Count) - segment.Offset;
            return System.Text.Encoding.UTF8.GetString(segment.Array, segment.Offset, count < 0 ? segment.Count : count);
        }

        internal static void Write(ArraySegment<byte> segment, byte value) => segment.Array[segment.Offset] = value;

        internal static void Write(ArraySegment<byte> segment, sbyte value) => segment.Array[segment.Offset] = (byte)value;

        internal static void Write(ArraySegment<byte> segment, ushort value)
        {
            segment.Array[segment.Offset] = (byte)value;
            segment.Array[segment.Offset + 1] = (byte)(value >> 8);
        }

        internal static void Write(ArraySegment<byte> segment, short value)
        {
            segment.Array[segment.Offset] = (byte)value;
            segment.Array[segment.Offset + 1] = (byte)(value >> 8);
        }

        internal static void Write(ArraySegment<byte> segment, uint value)
        {
            segment.Array[segment.Offset] = (byte)value;
            segment.Array[segment.Offset + 1] = (byte)(value >> 8);
            segment.Array[segment.Offset + 2] = (byte)(value >> 16);
            segment.Array[segment.Offset + 3] = (byte)(value >> 24);
        }

        internal static void Write(ArraySegment<byte> segment, int value)
        {
            segment.Array[segment.Offset] = (byte)value;
            segment.Array[segment.Offset + 1] = (byte)(value >> 8);
            segment.Array[segment.Offset + 2] = (byte)(value >> 16);
            segment.Array[segment.Offset + 3] = (byte)(value >> 24);
        }

        internal static void Write(ArraySegment<byte> segment, ulong value)
        {
            segment.Array[segment.Offset] = (byte)value;
            segment.Array[segment.Offset + 1] = (byte)(value >> 8);
            segment.Array[segment.Offset + 2] = (byte)(value >> 16);
            segment.Array[segment.Offset + 3] = (byte)(value >> 24);
            segment.Array[segment.Offset + 4] = (byte)(value >> 32);
            segment.Array[segment.Offset + 5] = (byte)(value >> 40);
            segment.Array[segment.Offset + 6] = (byte)(value >> 48);
            segment.Array[segment.Offset + 7] = (byte)(value >> 56);
        }

        internal static void Write(ArraySegment<byte> segment, long value)
        {
            segment.Array[segment.Offset] = (byte)value;
            segment.Array[segment.Offset + 1] = (byte)(value >> 8);
            segment.Array[segment.Offset + 2] = (byte)(value >> 16);
            segment.Array[segment.Offset + 3] = (byte)(value >> 24);
            segment.Array[segment.Offset + 4] = (byte)(value >> 32);
            segment.Array[segment.Offset + 5] = (byte)(value >> 40);
            segment.Array[segment.Offset + 6] = (byte)(value >> 48);
            segment.Array[segment.Offset + 7] = (byte)(value >> 56);
        }

        internal static unsafe void Write(ArraySegment<byte> segment, float value) => Write(segment, *(int*)&value);

        internal static unsafe void Write(ArraySegment<byte> segment, string value) =>
            System.Text.Encoding.UTF8.GetBytes(value, 0, Math.Min(value.Length, segment.Count), segment.Array, segment.Offset);

        internal static void Write<T>(ArraySegment<byte> segment, T[] values) where T : unmanaged
        {
            Buffer.BlockCopy(values, 0, segment.Array, segment.Offset, segment.Count);
        }

        internal static void Write<T>(ArraySegment<T> segment, T[] values)
        {
            Array.Copy(values, 0, segment.Array, segment.Offset, segment.Count);
        }
    }
}
