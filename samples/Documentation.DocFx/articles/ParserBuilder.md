## Parser Builder

```ParserBuilder<TOutput>``` type can be used to create message parsers with builder pattern. ParserBuilder can return three different type of Parsers depending on the selected ```TOutput``` and ```TInput``` generic type arguments.

1. MessageParser
2. TypedMessageParser<TTarget>
3. TypedStringMessageParser<TTarget>

When the selected output type argument is type of *object* a MessageParser will be created. Otherwise, depending on the input type argument, if it is *byte* ```TypedMessageParser<TOutput>```, if it is *char* a  ```TypedStringMessageParser<TOutput>``` will be returned.

### Parser Builder Modes

When a ParserBuilder is created it can be pre-configured by setting dependencies of the built parsers. This preconfigured ParserBuilder can be then shared across the application. When an actual parser is being built by any of the ```Build``` overload methods, the default configuration can be changed. During the build phase we can override or substitute missing dependencies of the parser built. While the pre-configured dependencies are available for all parser built, the dependencies overridden or substituted at build time will only be used for the actual parser. When no pre-configuration or build time dependencies are provided, each parser will be given its own dependencies per instance. 

#### Pre-configuration

The idea behind pre-configuration is that dependences that can be shared across all parser can be set once, during an initialization phase. For example, to share types registered to a ```TagToPropertyMapper```, we can pre-configure this dependency to a ```ParserBuilder``` instance. Then every parser instance created by this ParserBuilder will opt to use the pre-configured registrations, unless it is overridden at the build time. ParserBuilder has a ```SetXXXX``` method to pre-configure a given dependency. It also has an ```AddOutputType``` method to *Map* a type to the pre-configured ```TagToPropertyMapper```.

In the following example we pre-configure an *Order* output type with the ```TagToPropertyMapper``` and we build a ```MessageParser``` returning objects from input type of ```ReadOnlySpan<byte>```:
```csharp
var parser = new ParserBuilder<object>().AddOutputType<Order>().Build<byte>() as MessageParser;
```

#### Build phase

During the build phase we can override common parameters:
1. MessageParserOptions
2. ITagToPropertyMapper
3. Or all parameters

During the build phase, the output type of the ParserBuilder is always *mapped* with the ```TagToPropertyMapper``` being used.

In the following example we override the MessageParserOptions at build time:

```csharp
IMessageParser<Quote, char> parser = new ParserBuilder<Quote>().Build<char>(new MessageParserOptions() { SOHChar = '|' });
```



