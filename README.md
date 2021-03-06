# Fictional language generator
What is it?
------
Fictional language generator is a well tested API written in C# for construction strings with user defined order, length and content, based on frequency.


What it can be used for?
------
For generating sentences, characters names or other strings with certain patterns.


Whats idea behind it?
------
The general idea is to give user an API to create unique strings, by allowing them setting frequency for : 
<ul>
<li>length</li>
<li>order</li>
<li>amount of elements, this element consists of</li>
<li>element content (like string representation or other elements)</li>
</ul>

Updates
---------

*16/05/2017*

Chained syntax doesnt work.The problem isnt in fact that it can be "train crush",but more in fact that one method in 200 chracters long looks horrible. Thats why I decided to replace all chained methods. All methods,that marked as obselete will be changed/deleted in next commit, as tests will be redone with new syntax. Current version is not much better, but hey, there must be code somewhere. You have to use library as in `OrcLanguage` or `GettingStartedExample` example.

Getting started
------
### Theoretical part
<i style="color:grey;">Warning: this naming conventions may be bad. If you know better names for program entities - contact me.</i>

API consists of 2 basic entities which are divided in root and parent variants:
<ul>
<li><b>Syntactic property</b> is entity, set to difine order of certein element types. Root variant isnt much different from parent variant (and no different at all, if you only use API). Two difference are - root property contains lists of root syntactic units, while parent - parent syntactic units, and parent property contains info of what children its syntactic unit must have, to represent finished syntactic unit of its proprty.</li>
<li><b>Syntactic unit</b> is entity, that belong to certain syntactic property. Root syntactic unit contains string representation, while parent syntactic unit - possible children and children amount.</li>
</ul>

Lets clarify this by simple example - `consonant` is name for root syntactic property, and this property can start from property `vowel`. It contains  root syctactic units with string representations such as `w`,`r`,`t`. Root property `vowel` can go after property `consonant`, and contains units with string representaiton `a`,`i`,`u`. Then, parent syntactic property `word`, contains parent syntactic unit, which defines that word can be created from 3 children, and this possible children are properties `vowel` and `consonant`.<br/>
<br/>
And thats all you have to undestand, to use API. In this example we created simpliest tree. with parent `word` and children `vowel` and `consonant`. Engine will produce results such as `war`, `tir` and others.

Now lets see how its done by code.

### Practical part


OK, lets jump in code, creating out last example. First of all, lets create language constructor - its an object, through all interaction with engine goes.
```cs
LanguageConstructor languageConstructor = new LanguageConstructor();
```
Then, lets create our `consonant` property and specify order of that property:
```cs
IRootProperty consonant = languageConstructor.CreateRootProperty("consonant");
consonant.CanStartFrom(Pair.Of("Start", 100), Pair.Of("vowel", 100));
```
Ok, after creating  property `consonant`, we have to `CanStartFrom(string)` methods. `CanStartFrom("vowel", 100)` tells engine that property `consonant` can go after property `vowel` with frequency `100`. What this frequency is here for? Ok, lets say, we want that after `vowel` to go not only `consonant`, but other `vowel`, and this sequence `vowel`->`vowel` should be constructed like 10 times less often, then usual `consonant`->`vowel`. Lets write this.

```cs
IRootProperty vowel = languageConstructor.CreateRootProperty("vowel");
vowel.CanStartFrom(Pair.Of("consonant", 100), Pair.Of("vowel", 10));
```
Done.

There is still 1 unclear element. In creating property `consonant` we written `CanStartFrom("Start", 100)`. Start property is first property added to construction scheme, and we said that `consonant` should go after it. **Language construction must have properties that can go after `Start` property.**

Now we know how specify ordering and its order frequency, but we havent created any syntactic units, to construct our words. Lets declare some consonants.

```cs
languageConstructor.CreateRootSyntacticUnitsOfProperty(consonant, Pair.Of("w", 100), Pair.Of("r", 100), Pair.Of("t", 100));
```
We declared three `consonant`s, each of them will be met with same frequency - `100`. What if we want to have letter `j` that will be met like 5 times more, than any other consonant.
```cs
languageConstructor.CreateRootSyntacticUnitsOfProperty(consonant, Pair.Of("j", 500));
```
OK, lets create some vowels:
```cs
languageConstructor.CreateRootSyntacticUnitsOfProperty(vowel, Pair.Of("a", 100), Pair.Of("o", 100), Pair.Of("u", 100));
```
Here is nothing new, we declared three syntactic units of property `vowel`, which will be met with same frequency. Now lets create parent property with name `word`.
```cs
IParentProperty word = languageConstructor.CreateParentProperty("word");
word.CanStartFrom("Start", 100);
```
Difference between `CreateParentProperty(string)` and `CreateRootProperty(string)` is that parent property reference parent syntactic units, which can have as children any parent property or root property. Also, note that we again told engine that `word` can start from property `Start`. Lets create parent syntactic units and specify its children.

For creating parent syntactic unit we need to specify its children, and its children amount.
```cs
IParentSU wordSu = languageConstructor.CreateParentSyntacticUnit("word");
wordSu.AddPossibleChild(Pair.Of("consonant", 100), Pair.Of("vowel", 80));
wordSu.AddChildrenAmount(3, 100);
```
Here we told engine that syntactic unit that belong to property `word` should have children `consonant` and `vowel` (`AddPossibleChild("consonant", 100)`, `AddPossibleChild("vowel", 80)`). Child `vowel` will be chosen in 80/180 cases (total frequency 180 is sum of: `80` frequency of `vowel`, `100` frequency of `consonant`). This syntactic unit will have exatly 3 children, which is specified by method `AddChildrenAmount(3, 100)`. 

We finished creating of our basic graph. Now, only thing that left is to get results:
```cs
languageConstructor.GetStringListOfProprety("word", 10);
```
And all is done. Example output listed is below:
```
juj
juj
joj
tuj
wuj
tot
jaj
wat
ruj
joj
```
Not the most incredible result, but even now, we created sequence of words that are remarkable by wast amount of letter `j` in it. Code used in this example can be found at <a href = "https://github.com/shinigamixas/Fictional-Language-Generator/blob/master/LanguageGenerator.UsageExamples/Examples/GettingStartedExample.cs">this link</a>. If you want more, welcome to predefined sets section. 

API overview (class `LanguageConstructor`) 
------
This topic mainly describes class `LanguageConstructor`.
### Properties
Principal of properties is in core of engine. Properties can be seen as type of syntactic units. Properties define order. Parent property also define what syntactic units of its type must contain to be complete. Also, properties can define how frequent certain property will be used, by specifing its frequency in `CanStartFrom(...)` method, if there is more than property, that can go after a property.
#### Default properties
Currently engine contains 2 syntactic units by default. 

Its unit with property named `Start`, that represents start of new construction, made to clarify for engine, what properties follow start of construction. When setting order property can be referenced by its string name `"Start"` or: 
```cs
BasicSyntacticUnitsSingleton.StartOfConstructionProperty
```

And unit with property name `Any`, that made to make user properties to follow any other properties. Can be referenced by its string name `"Any"` or: 
```cs
BasicSyntacticUnitsSingleton.AnyProperty
```
#### Creating properties
In code snippets below methods belong to class `LanguageConstructor`. Creating properties through language constructor will automatically add them in constructor repository.
##### Creating root property
`string propertyName` is name of property, which will be referenced by other syntactic units.
```cs
IRootProperty CreateRootProperty(string propertyName)
```
##### Creating parent property
`string propertyName` is name of property, which will be referenced by other syntactic units.
```cs
IParentProperty CreateParentProperty(string propertyName)
```
#### Specifying order
Property with `propertyNameToStartFrom` may not exist yet, its can be declayared later. Method adds `propertyNameToStartFrom` to collection , which later will be used to link data, or you can use exising link of property.
```cs
T CanStartFrom<T>(this T property, string propertyToGoAfter, int withFrequency = 100) where T: IProperty { }
T CanStartFrom<T>(this T property, IProperty propertyToGoAfter, int withFrequency = 100) where T : IProperty { }
void CanStartFrom<T>(this T property, params KeyValuePair<IProperty, int>[] propertiesToStartFrom) where T : IProperty { }
void CanStartFrom<T>(this T property, params KeyValuePair<string, int>[] propertiesToStartFrom) where T : IProperty { }
```
#### Setting child properties that parent property must contain
`IParentProperty` is derived from `IPropertyMustContainInfoForLinker`. There are few things to keep in mind when using this method. You can specify few different or same properties, all of which will be constructed. And *must have* properties will be constructed at first possible place, based on their order. If not all children properties will be used during construction - exception will be thrown. Also, parent property must contain enogh space tp all *must have* children.
```cs
T MustContainProperty<T>(this T mustContainInfo, string propetyNameToContain) where T : IPropertyMustContainInfoForLinker
```
### Syntactic units
One main rule: **create property before its syntactic units**. You can reference not existing properties in when setting order and children by their future names, but you cant create syntactic unit, and reference it to not existing property by its string name.
#### Creating root syntactic unit
Two methods can be used to create `IRootSU` (SU from Syntactic Unit). As usual, method overload with `IRootProperty` argument, if you dont want to use string name (`itsPropertyName`).
```csharp
IRootSU CreateRootSyntacticUnit(string stringRepresentation, string itsPropertyName, int frequency = 100)
IRootSU CreateRootSyntacticUnit(string stringRepresentation, IRootProperty itsProperty, int frequency = 100)
```
Also there is method, which takes last created instance or IRootProperty, and uses it as argument to two methods above. Must be used right after creation of IRootProperty.
```cs
IRootSU CreateRootSyntacticUnitWithLastCreatedProperty(string stringRepresentation, int frequency = 100)
```
#### Creating parent syntactic unit
These two methods can be used to create `IParentSU`. After creation at least one amount of children and possible childe needs to be specified, to `IParentSU` to work.
```cs
IParentSU CreateParentSyntacticUnit(string itsPropertyName, int frequency = 100)
IParentSU CreateParentSyntacticUnit(IParentProperty itsProperty, int frequency = 100) 
```
#### Specifying possible children and amount of children
Child can be specified in two methods, direct link and string propety name. Interface `IChildInfoForLinker` implemented in `IParentSU`. Possible child can be as `IRootProperty` as `IParentProperty`, which can by himself be constructed of many children.
```cs
T AddPossibleChild<T>(this T parentSU, IProperty property, int frequencyForThatAmount = 100) where T : IParentSU { }
T AddPossibleChild<T>(this T childInfo, string propertyName, int frequencyForThatAmount = 100) where T : IChildInfoForLinker { }
void AddPossibleChild<T>(this T childInfo, params KeyValuePair<string, int>[] propertyNamesAndFrequencyPairs) where T : IChildInfoForLinker { }
void AddPossibleChildren<T>(this T parentSU, params KeyValuePair<IProperty, int>[] propertyAndfrequencyPairs) where T : IParentSU { }
```
Amount of children method. 
```cs
T AddChildrenAmount<T>(this T parentSU, int amount, int frequencyForThatAmount = 100) where T : IParentSU
void AddChildrenAmount<T>(this T parentSU, params KeyValuePair<int, int>[] amountAndFrequencyKeyValuePairs) where T : IParentSU
```
### Getting result string
You can get one result string by those methods.
```cs
string GetResultStringOfProperty(string propertyName) { }
string GetResultStringOfProperty(IProperty property) { }
```
Or, if you want many result strings, you can use:
```cs
IEnumerable<string> GetStringEnumerableOfProperty(this ISyntacticUnitConstructor constructor, string propertyName, int amountOfStrings)
```
Which will be using multiple threads for large `amountOfStrings`. 

Examples of usage
------
Now, to explore engine more, lets create graph that will be used for simulating orc sentences. Full code can be found <a href = "https://github.com/shinigamixas/Fictional-Language-Generator/blob/master/LanguageGenerator.UsageExamples/Examples/OrcLanguage.cs">here</a>.

First of all, lets define `languageConstructor`. 
```cs
LanguageConstructor languageConstructor = new LanguageConstructor();
```
Now lets define root properties. Here is defined root property `consonant` and its order.
```cs
IRootProperty consonant = languageConstructor.CreateRootProperty("consonant");
consonant.CanStartFrom(Pair.Of("Start", 100), Pair.Of("whitespace", 100), Pair.Of("consonant", 15), Pair.Of("vowel", 100));
```
Next, lets difine syntactic units of property `consonant`, with frequency, how often certain consonant sound met. As we want to orcish language sound rough, such letters as `r`,`w` has bigger frequency.
```cs
languageConstructor.CreateRootSyntacticUnitsOfProperty(
                consonant,
                Pair.Of("w", 150),
                Pair.Of("r", 180),
                ...
                Pair.Of("c", 110));
```
Next on the list `vowel` property. As we want to have squence `vowel`->`vowel` met not to often, as its makes language *lighter*, frequency for this case only `5`.
```cs
 IRootProperty vowel = languageConstructor.CreateRootProperty("vowel");
vowel.CanStartFrom(Pair.Of("Start", 20), Pair.Of("whitespace", 20), Pair.Of("consonant", 100), Pair.Of("vowel", 5));
```
Defining `vowel` syntactic units.
```cs
languageConstructor.CreateRootSyntacticUnitsOfProperty(
                vowel,
                Pair.Of("a", 100),
                Pair.Of("u", 100),
                Pair.Of("o", 100),
                Pair.Of("i", 40),
                Pair.Of("e", 100));
```
Ok, after we difined all sounds, lets define words.
```cs
IParentProperty orcishWord = languageConstructor.CreateParentProperty("orcish word");
orcishWord.CanStartFrom(Pair.Of("Start", 100), Pair.Of("whitespace", 100));
```
Now, lets create long variant of word
```cs
IParentSU longOrcishWordSU = languageConstructor.CreateParentSyntacticUnit(orcishWord);
longOrcishWordSU.AddPossibleChildren(Pair.Of(consonant, 100), Pair.Of(vowel, 100));
longOrcishWordSU.AddChildrenAmount(Pair.Of(5, 100), Pair.Of(6, 150), Pair.Of(7, 130));
```
And short variant, that will be met more often.
```cs
IParentSU shortOrcishWordSU = languageConstructor.CreateParentSyntacticUnit(orcishWord, 160);
shortOrcishWordSU.AddPossibleChildren(Pair.Of(consonant, 100), Pair.Of(vowel, 100));
shortOrcishWordSU.AddChildrenAmount(Pair.Of(3, 150), Pair.Of(4, 130));
```
Previosly, we used property `whitespace` a lot, now lets define it with its syntactic unit and string representation.
```cs
IRootProperty whitespace = languageConstructor.CreateRootProperty("whitespace");
whitespace.CanStartFrom(orcishWord);
languageConstructor.CreateRootSyntacticUnitsOfProperty(whitespace, Pair.Of(" ", 100), Pair.Of(", ", 30), Pair.Of(": ", 5));
```
You can see that whitespace can also be with coma or colon.

And now we ready to define sentence base. Notice how many children we set for this syntactic unit.
```cs
IParentProperty orcisgSentenceBase = languageConstructor.CreateParentProperty("orcish sentence base");
orcisgSentenceBase.CanStartFrom("Start", 100);
IParentSU orcishSentenceBaseSU = languageConstructor.CreateParentSyntacticUnit(orcisgSentenceBase);
orcishSentenceBaseSU.AddPossibleChildren(Pair.Of(orcishWord, 100), Pair.Of(whitespace, 100));
orcishSentenceBaseSU.AddChildrenAmount(Pair.Of(3, 120), Pair.Of(5, 110), Pair.Of(7, 100), Pair.Of(9, 100));
```
Why only odd number of children? Very simple - adding only odd number we ensure that sentence base will end in `orcish word`,and not in `whitespace`, as only `orcish word` can start from `start`, and can be followed only by `whitespace`. And `whitespace` can be followed only by `orcish word`. Now possible sequences are:
* `Start`->`orcish word`->`whitespace`->`orcish word`
* `Start`->`orcish word`->`whitespace`->`orcish word`->`whitespace`->`orcish word`
* `Start`->`orcish word`->`whitespace`->`orcish word`->`whitespace`->`orcish word`->`whitespace`->`orcish word`
* `Start`->`orcish word`->`whitespace`->`orcish word`->`whitespace`->`orcish word`->`whitespace`->`orcish word`->`whitespace`->`orcish word`

Now, when we sure that our `orcish sentence base` will end in `orcish word`, we can create `sentence ending`.
```cs
IRootProperty sentenceEnding = languageConstructor.CreateRootProperty("sentence ending");
sentenceEnding.CanStartFrom(orcisgSentenceBase);
languageConstructor.CreateRootSyntacticUnitsOfProperty(
                sentenceEnding,
                Pair.Of(".", 100),
                Pair.Of("!", 50),
                Pair.Of("?", 50),
                Pair.Of("?!", 70),
                Pair.Of("!!!", 50));
```
All we left to do is join `orcish sentence base` and `sentence ending` in one structure.
```cs
IParentProperty orcishSentence = languageConstructor.CreateParentProperty("orcish sentence");
orcishSentence.CanStartFrom("Start", 100);
IParentSU orcishSentenceSU = languageConstructor.CreateParentSyntacticUnit(orcishSentence);
orcishSentenceSU.AddPossibleChildren(Pair.Of(orcisgSentenceBase, 100), Pair.Of(sentenceEnding, 100));
orcishSentenceSU.AddChildrenAmount(2, 1);
```
Thats all. Our orc sentence generator ready. Lets use it.
```cs
languageConstructor.GetStringEnumerableOfProperty("orcish sentence", 20);
```
And the orcish output.
```cs
Doegul avi kab bzanaku? 
Dok mkewefo wow!!! 
Kod uwa, vun?! 
Gonogit zozena suw doki?! 
Sekado uba wonar viwizu zauzaze! 
Zle hoz adb liriza rraztb. 
Used awab!!! 
Dow, dahravu vil gato. 
Oto roburah kuv, dozbuo. 
Sun daw twow bwurob! 
Kopagis kad pozupe waw koav. 
Rav rel heob! 
Row wak. 
Nur rugder rezvlov?! 
Duho, poro vedo tukuw. 
Datoddu, zafud!!! 
Wabtif: lim, buwv! 
Ozekiti ksg gev waz. 
Pet kmbuliv noll efup. 
Hupalo fov.
```
Not bad, huh? In ~60 lines of code we created near infinite amount of orcish senteces, which can be randomly generated at any time in your game or app. (Note: in output first letter is made uppercase by .net not language generator related code, initially all letters where lowercase).


Predefined sets
------

<a href = "https://github.com/shinigamixas/Fictional-Language-Generator/blob/master/LanguageGenerator.UsageExamples/Examples/OrcLanguage.cs">Orc language senteces.</a>
Example output:
```
Wegor ohe bat rade buche!!!
Kud var.
Chos ked ropepah!!!
Uhevoza nubo hazevz gud, hepo.
Tew, uchuv bapuve?!
Zafe rreh?!
Wote buru, ruranap!
Tuk puwel, chur, used kawosuch?
Ruh dorir awed.
Sud, bobe kore veb ahz?!
```
<a href = "https://github.com/shinigamixas/Fictional-Language-Generator/blob/master/LanguageGenerator.UsageExamples/Examples/ElvenLanguage.cs">Elven language senteces.</a>
Example output:
```
Lakawus uhumauv.
Elow faru-nipup, uge?!
Hibel-epalum ikee, depuwug.
Ail wavehu, ahifdiw, aeuibe, miwi.
Munite: volei fateput.
Uhifoti oseke.
Resaf wisi.
Hafeva afekip adu, echechil?
Avizu upeiit, ineka.
Kupivoch igilasu, miwigii enene.
```
<a href = "https://github.com/shinigamixas/Fictional-Language-Generator/blob/master/LanguageGenerator.UsageExamples/Examples/FictionalEnglishLikeLanguage.cs">English-like senteces.</a>
Example output:
```
Tochuse eha, adie etudmoh-riheih?
Wexota etohae ohuga.
Oetvi htise, egidesa.
Regochaa, net vega?
Gecirfe cerchuh yemoto.
Welawae atosi pocihh.
Meriw eduha.
Esijear slnin: tigroer-omee.
Oaataw ehatos niroguch sina iechsavch.
Tdahaf detalo, emgeco neey.
```

