## Message Builder

Message builders are available for building FIX messages. Currently the library supports a low-level ```MessageBuilder```, which means it can build messages from *string* and *int* typed parameters, but it cannot build a message from POCO classes.

```MessageBuilder``` is designed to be re-used. It means that after a message is built, the same instance can be used to build further messages. For all message, it remembers the maximum length of them. For the next message to be built it allocates arrays with the using this value. This way when message is being built, it can avoid unnecessary array copies, making the whole process faster.

>Note, that currently ```MessageBuilder``` does not support building messages with raw byte data as a value for a tag.

### Building messages

#### Adding tags and values

```MessageBuilder``` provides a couple of overloads add individual tags to a message. It does a very basic validation on the input message only.

We can add (with ```AddTag``` method) a tag and value by providing parameters tag as an integer and value as a string. We can also provide custom message encoding to the value. Once a message encoding is chosen for a message, it cannot be changed. We can also provide a single string parameter for the tag and the value as ```23=CustomValue|```. In this case use '=' *char* to separate the tag from the value and '|' *char* to indicate the SOH byte. If any of these characters are missing an ```ArgumentException``` is thrown. These methods can be chained with fluent syntax so a more comples message can be built.

```csharp
byte[] message0 = new MessageBuilder().AddTag(23, "CustomValue").Build();
//or
byte[] message1 = new MessageBuilder().AddTag("23=CustomValue|").Build();
```

Finally, an ```AddRaw``` method is available to provide multiple tags and values within a single string, for example: ```51=23|23=CustomValue|```. In this case no validation is happening, and '|' *char* has to be used as the SOH character.

```csharp
byte[] message = new MessageBuilder().AddRaw("23=CustomValue0|24=CustomValue1|").Build();
```

When building a message, we can also define the ```FixVersion``` to be used by setting with the ```AddBeginString``` method.

```csharp
byte[] message = new MessageBuilder().AddBeginString(SupportedFixVersion.Fix42).AddTag(23, "CustomValue").Build();
```

#### Build phase

During the build phase the ```MessageBuilder``` assembles the rest of the message by prepending the version and the length tags and by calculating and suffixing the message with the checksum.

There are two overloads of the Build method. The first has no input arguments and returns a *byte[]*. This method allocates a byte array with the exact size of the message for each message.

> Note, that the ```MessageBuilder``` may internally use further arrays to maintain the message being built, but these are cached.

The second overload has a ```Span<byte>``` input argument and a *int* return type. In this case the span is filled with the message, and the length of the message is returned by the method.

```csharp
Span<byte> message = stackalloc byte[40];
int length = new MessageBuilder().AddTag(23, "CustomValue").Build(message);
message = message.Slice(0, length);
```

### Extending the MessageBuilder

```MessageBuilder``` has protected methods to override for extension. All of them are invoked during the build phase in the given order:
* CalculateRequiredSize to calculate the byte size of the message required (when a *byte[]* is being returned)
* AddVersion to override the default FixVersion
* AddLength to override the default length
* AddChecksum to override the default checksum
* Clear to reset the state of the message builder once the message is built