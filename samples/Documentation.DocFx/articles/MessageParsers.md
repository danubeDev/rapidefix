## Message Parsers

This article details how to get started with custom message parsers, what type of message parsers we can build, and what dependencies we need to pass to create message parsers.

The goal of a message parser is to validate and parse a single message from the message type tag to the checksum tag. It creates the desired output type instance and fills the property values according to input message and ```[FixTag]``` attributes on the output type.

### Types of Message Parsers

Message parsers implement an interface ```IMessageParser<TOutput, TData>```, where TOutput is the type returned by the parser and TData is the type of the input (*char* or *byte*). 

There are three different type of message parser
1. MessageParser
2. TypedMessageParser<TTarget>
3. TypedStringMessageParser<TTarget>

#### MessageParser

The message parser is the most generic parser, as it implements ```IMessageParser<object, byte>```. It is designed to accept a byte span and return an object. The runtime type of the returned object is decided by the parser using the ```MessageTypeAttribute``` of the registered output types.
If it is used by registering only a single type (or the actual type of the object is known by the user), convenient overloads for ```Parse``` exist to return type ```T``` as the return type.

The parser validates the input message, determines the output type, traverses the input message and tries to parse the tags and set it on the output object. If the object is known by the user, it can be provided as an input argument.

We can create a MessageParser by using the ```ParserBuilder``` on objects and adding our custom output types. In the example below only one Order output type is given.

```csharp
new ParserBuilder<object>().AddOutputType<Order>().Build<byte>() as MessageParser;
```

We can also create one by manually providing the dependencies:

```csharp
// Create a property mapper and map types to be parsed. SubPropertySetterFactory is responsible creating the actual property setters.
var propertyMapper = new TagToPropertyMapper(new SubPropertySetterFactory());
propertyMapper.Map<Order>();
// Create the composite property setter. CompositePropertySetter is the delegator of the sub property setters.
var compositeSetter = new CompositePropertySetter();
// Create a validator collection to have all default validators
var validators = new ValidatorCollecti(IntegerToFixConverter.Instance)
// Passing empty options
var options = new MessageParserOptions();
// Create MessageParser, passing dependencies
var parser = new MessageParser(propertyMapper, compositeSettervalidators, options);
```

> MessageParser does not support parsing into value types, it is a *wildcard* parser for reference types, if the fix tags are of the types are disjunct.

MessageParser can be configured to return null or throw an ```ArgumentException``` in case of a validation error. This behavior can be customized through the *ThrowIfInvalidMessage* property of ```MessageParserOptions```

#### TypedMessageParser

```TypedMessageParser<TTarget>``` is a parser which can parse messages into type of ```TTarger```. It cannot parse wildcard types, but it can parse value and reference types at the same time.

Creating a TypedMessageParser is similar to MessageParser. We can either create it by ```ParserBuilder``` or we can pass the dependencies our self. Creating it by parser builder:

```csharp
IMessageParser<Order, byte> parser = new ParserBuilder<Order>().Build<byte>();
```

#### TypedStringMessageParser

```TypedStringMessageParser<TTarget>``` is a specialized message parser with input type of span *char* and out output type of ```TTarget```. It skips validation as it does not apply for char input, hence it is faster for this case. Creating a TypedStringMessageParser by ```ParserBuilder```:

```csharp
IMessageParser<Quote, char> parser = new ParserBuilder<Quote>().Build<char>(new MessageParserOptions() { SOHChar = '|' });
```

Note, that in the above case we push the SOH charactor as ```|``` a pipe character. To do this, the MessageParserOptions's SOHChar property is set.
