# RapideFix
A superb FIX message parser library.

### What's the area?
FIX messages: Financial world has standard to define the interface of pre-trade and trade communications, FIX (https://en.wikipedia.org/wiki/Financial_Information_eXchange). Billions of these messages are exchanged every day. Considering the amount of messages can arrive to a trading application/engine, it is not hard to say, we are going to end up with enormous amount of data, where parsing efficiently is crucial.

#### What's the main purpose?
Save unnecessary memory allocations: Generally, the application will have to parse the string stream FIX message to it's internal representation. Parsing such a string stream, most of the time, results a significant amount of substring operation. From the nature of string in C#, as a consequence, there will be a big amount of temporary string object. It is not ideal, as we could avoid to create such temporary data, assuming that the incoming message we are actually parsing won't change.

### How to achieve this?
Using Span<T> and Memory<T>: Take advantage of newly introduced structures in C#, Span<T> and Memory<T>. At every possible cases, where a substring, subset of indexible immutable collection of elements needed, use these structures to avoid the overhead of creating the subset. Instead wrap them into the advanced structure, that will provide an easy interface and hide the logical complexity of dealing with pointers, references.
