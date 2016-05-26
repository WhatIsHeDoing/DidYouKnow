#include <algorithm>
#include <cassert>
#include <cstdlib>
#include <iostream>
#include <iterator>
#include <list>
#include <map>
#include <sstream>
#include <stdexcept>
#include <string>
#include <typeinfo>
#include <vector>

#ifdef TEST_PLACEHOLDER_EXAMPLE
/**
 * Macros are generally ill-advised, but among their uses, are great
 * for code snippets you would like to compile, but never run,
 * for demonstration purposes, particularly if a huge comment
 * block would likely be deleted by a well-meaning engineer
 */
void testPlaceholder ()
{
    assert(0);
}
#endif

/**
 * Demonstrates that the body of an if statement will run
 * if the variable declared in the check is not falsy
 */
void testBranchOnVariableDeclaration ()
{
    std::map<std::string, std::string> data;
    data["key"] = "value";

    if (std::string * value = &data["key"])
    {
        assert(* value == "value");
        return;
    }

    assert(0);
}

/**
 * Shows the many methods that can be used to access an index of an array.
 * Yes, array[index] is syntactic sugar!
 */
void testArrayIndexAccess ()
{
    int array[3];
    array[0] = 0;
    array[1] = 1;
    array[2] = 64;

    assert(array[2] == 64);
    assert(*(array + 2) == 64);
    assert(*(2 + array) == 64);
    assert(2[array] == 64);
}

/**
 * Originally implemented for limited keyboards and terminals, why not
 * clarify or obfuscate your codebase with these plain-text alternatives?
 */
void testKeywordOperatorTokens ()
{
    assert(1 and 1);
    assert(0 or 1);

    int array[1];
    array[0] = 5;
    assert(array<:0:> == 5);

    if (1)
    <%
        assert(1);
        return;
    %>

    assert(0);
}

class ContainsHidden
{
public:
    ContainsHidden (const int member) : _member(member) { }

protected:
    const int _member;
};

class PromotesHidden : public ContainsHidden
{
public:
    PromotesHidden (int member) : ContainsHidden(member) { }
    using ContainsHidden::_member;
};

/**
 * Demonstrates how to alter the scope of class members in their derived classes
 */
void testChangingScope ()
{
    assert(PromotesHidden(5)._member == 5);
}

#define private public
class NotPrivateHere
{
private:
    bool success () const
    {
        return true;
    }
};
#undef private

/**
 * Shows the highly-questionable practice of redeclaring language keywords
 * with macros, which can - very occasionally - be useful when testing
 */
void testRedefiningKeywords ()
{
    assert(NotPrivateHere().success());
}

struct PointToUs {
    int value;

    bool method () const
    {
        return true;
    }
};

/**
 * Demonstrates the syntax required for pointers to function members,
 * which - when required - may be a pain to remember quickly
 */
void testPointerToMemberOperators ()
{
    int PointToUs::*valuePointer = &PointToUs::value;
    bool (PointToUs::*methodPointer)() const = &PointToUs::method;

    PointToUs stack;
    PointToUs * heap = new PointToUs;

    assert((stack.*methodPointer)());
    assert((heap->*methodPointer)());

    stack.*valuePointer = 1;
    assert(stack.*valuePointer == 1);

    heap->*valuePointer = 2;
    assert(heap->*valuePointer == 2);

    delete heap;
}

struct BaseWithHiddenData {
    BaseWithHiddenData (const int data) : _data(data) { }

protected:
    const int _data;
};

struct DerivedExposesHiddenData : BaseWithHiddenData
{
    static int get (BaseWithHiddenData & baseWithHiddenMember)
    {
        return baseWithHiddenMember.*(& DerivedExposesHiddenData::_data);
    }
};

/**
 * Shows how those member pointers can be used for the more questionable
 * practice of exposing hidden class members
 */
void testMemberPointersCircumventScope ()
{
    BaseWithHiddenData baseWithHiddenData(31313);
    assert(DerivedExposesHiddenData::get(baseWithHiddenData) == 31313);
}

std::string letMeKeepMyReturnValue ()
{
    return "value";
}

void passByRefAlternative (std::string & data)
{
    data = "value";
}

/**
 * Why use pass-by-reference to return huge data, when consumers can store
 * constant references to the returned values instead?
 */
void testScopeGuardTrick ()
{
    const std::string & keptReturnValue = letMeKeepMyReturnValue();
    std::string fromRef;
    passByRefAlternative(fromRef);
    assert(keptReturnValue == fromRef);
}

class PrePostInDecrementOverloading {
private:
    int _value;

    void incrementValue ()
    {
        _value = _value + 1;
    }

    void decrementValue ()
    {
        _value = _value - 1;
    }

public:
    PrePostInDecrementOverloading () : _value(0) { }

    int getValue () const
    {
        return _value;
    }

    // pre
    PrePostInDecrementOverloading & operator++ (int value)
    {
        incrementValue();
        return * this;
    }

    PrePostInDecrementOverloading & operator-- (int value)
    {
        decrementValue();
        return * this;
    }

    // post
    PrePostInDecrementOverloading & operator++ ()
    {
        incrementValue();
        return * this;
    }

    PrePostInDecrementOverloading & operator-- ()
    {
        decrementValue();
        return * this;
    }
};

/**
 * Highlights the subtle differences required for defining
 * pre and post-in/decrement operator overloading
 */
void testPrePostInDecrementOverloading ()
{
    PrePostInDecrementOverloading test;
    assert(test++.getValue() == 1);
    assert(test--.getValue() == 0);
    assert((++test).getValue() == 1);
    assert((--test).getValue() == 0);
}

template<template<class, class> class V, class T>
class CreateContainer
{
protected:
    V< T, std::allocator<T> > _container;

public:
    CreateContainer & addValue (const T & value)
    {
        _container.push_back(value);
        return * this;
    }

    CreateContainer () { }

    CreateContainer (const T & value)
    {
        addValue(value);
    }

    CreateContainer & operator, (const T & value)
    {
        return addValue(value);
    }

    CreateContainer & operator() (const T & value)
    {
        return addValue(value);
    }

    V< T, std::allocator<T> > get () const
    {
        return _container;
    }
};

/**
 * Demonstrates how overloading the brackets and comma operators
 * can ensure your interfaces are consumed more effectively.
 * These operate as fluent methods, returning a reference to the object
 * so that calls to it may be chained.
 */
void testFluentCommaAndBracketOverloads ()
{
    std::vector<int> integers;
    integers.push_back(0);
    integers.push_back(1);
    integers.push_back(2);

    const std::vector<int> & constIntegers
        = (CreateContainer<std::vector, int>(0), 1, 2).get();

    assert(constIntegers == integers);

    std::list<std::string> strings;
    strings.push_back("hello");
    strings.push_back("world");

    assert((CreateContainer<std::list, std::string>
        ("hello")("world").get()) == strings);
}

struct ReturnOverload
{
    ReturnOverload () { }

    int get ()
    {
        return 5;
    }

    std::string get () const
    {
        return "hello";
    }
};

/**
 * Wait, a function signature can be overridden via only the returned type?
 * Yes, only if it is marked as const, and therefore only invoked on constants.
 * Probably most commonly used to return a const_iterator instead of a
 * mutable iterator on constants.
 */
void testReturnOverload ()
{
    ReturnOverload returnOverload;
    const int & i = returnOverload.get();
    assert(i == 5);

    const ReturnOverload constReturnOverload;
    const std::string & s = constReturnOverload.get();
    assert(s == "hello");
}

namespace ThisNamespace
{
    bool HasThisFunction ()
    {
        return true;
    }

    namespace SubNamespace
    {
        std::string HasThisFunction ()
        {
            return "Yes, it does!";
        }
    }
}

struct ThisClass
{
    static int HasThisFunction ()
    {
        return 4;
    }

    typedef bool HasThisTypedef;
};

/**
 * Shows how namespacing is effectively used to access static class methods
 * and typedefs, and when aliasing a namespace can help simplify access
 */
void testNamespaces ()
{
    assert(ThisNamespace::HasThisFunction());
    assert(ThisClass().HasThisFunction() == ThisClass::HasThisFunction());
    assert(ThisClass::HasThisTypedef(true));

    namespace alias = ThisNamespace::SubNamespace;

    assert(ThisNamespace::SubNamespace::HasThisFunction()
        == alias::HasThisFunction());
}

bool ternaryTrue ()
{
    return true;
}

bool ternaryFalse ()
{
  return false;
}

/**
 * Ternary statements are mainly used for initialising constants
 * based on conditions, but they can also be used as lvalues!
 */
void testTernaryAsValue ()
{
    int postiveCount(0);
    int negativeCount(0);
    (5 < 0 ? negativeCount : postiveCount)++;
    assert(postiveCount == 1);
    assert(negativeCount == 0);

    assert((1 ? ternaryTrue : ternaryFalse)());
    assert(!(0 ? ternaryTrue : ternaryFalse)());

    try {
        const int i = (false) ? 5 : throw std::string("oops");
    }
    catch (const std::string & e)
    {
        assert(e == "oops");
        return;
    }

    assert(false);
}

/**
 * Gotos are dead, but long live URI labels?
 */
void testBareURIViaGoto ()
{
    goto http;
    assert(0);
    return;

    http://test.com
    assert(1);
}

/**
 * A hack that uses an ellipsis - which represents a variable
 * number of parameters to a function - to catch any exception
 */
void testCatchAnyException ()
{
    try
    {
        throw new std::string("badness");
    }
    catch (...)
    {
       assert(1);
       return;
    }

    assert(0);
}

struct ClassWithFunction
{
    int function ()
    {
        return 0;
    }
};

struct ClassWithoutFunction { };

template <typename T>
class HasFunction
{
    typedef char yes;
    typedef long no;

    template <typename C>
    static yes test (typeof(&C::function));

    template <typename C>
    static no test (...);

public:
    enum
    {
        exists = (sizeof(test<T>(0)) == sizeof(char))
    };
};

/**
 * An interesting templated approach to determining
 * whether a function exists within a class
 */
void testTemplateChecksFunctionExists ()
{
    assert(HasFunction<ClassWithFunction>::exists);
    assert(!HasFunction<ClassWithoutFunction>::exists);
}

template<typename T> 
struct ID
{
    typedef T type;
};

bool calledViaIdentityDefinition ()
{
    return true;
}

/**
 * Uses a class that lets you declare use a function as a variable
 * or parameter in a way that may be easier to read than the standard syntax.
 * See also boost::identity.
 */
void testIdentityMetaFunction ()
{
    ID<bool()>::type * IDFunc = & calledViaIdentityDefinition;
    assert(IDFunc());
}

template<typename T>
bool needUnaryOperatorForArraysHere (T const & a, T const & b)
{
    return true;
}

/**
 * The unary operator is useful for cases such as this test, where the template
 * type reference would be different for two arrays of differing lengths passed
 * to a function, so it decays them to their data type, which will work instead!
 */
void testDecayArrayToPointerViaUnaryOperator ()
{
    int smallerArray[2];
    int largerArray[3];
    assert(needUnaryOperatorForArraysHere(+smallerArray, +largerArray));
}

template<typename FunctionOne, typename FunctionTwo>
class CallsSurrogates {
    FunctionOne * _functionOne;
    FunctionTwo * _functionTwo;

public:
    CallsSurrogates(FunctionOne * functionOne, FunctionTwo * functionTwo)
        : _functionOne(functionOne), _functionTwo(functionTwo) { }

    operator FunctionOne * ()
    {
        return _functionOne;
    }

    operator FunctionTwo * ()
    {
        return _functionTwo;
    }
};

std::string integerFunction (int i)
{
    return "integer passed";
}

std::string longFunction (long l)
{
    return "long passed";
}

/**
 * Shows how the function templates used in the signature of a class
 * are called as surrogates to effectively operate as overloads.
 * This is achieved thanks to SFINAE (Substitution Failure is Not an Error).
 */
void testCallSurrogateFunctions ()
{
    CallsSurrogates<std::string(int), std::string(long)>
        callsSurrogates(integerFunction, longFunction);

    assert(callsSurrogates(5) == "integer passed");
    assert(callsSurrogates(5L) == "long passed");
}

void voidReturn () { }

/**
 * A demonstration of how a function that returns void
 * can be called and returned in one statement
 */
void testVoidReturn ()
{
#ifdef ALTERNATIVE
    if (1)
    {
        voidReturn();
        return;
    }
#endif

    return voidReturn();
}

#define COMPILE_TIME_TYPE(type) #type

template <typename T>
struct FindMyType { };

/**
 * Shows the difference between finding the type name of a variable
 * via a compile-time macro that uses the stringification operator,
 * versus the runtime version, which may return platform-dependent results
 */
void testFindingTypeName ()
{
    assert(COMPILE_TIME_TYPE(FindMyType<int>()) == "FindMyType<int>()");

    assert(COMPILE_TIME_TYPE(FindMyType<int>())
        != typeid(FindMyType<int>()).name());
}

bool functionTryBlockEntered = false;

int throwingFunction ()
{
    throw true;
}

struct ClassWithFunctionTryBlock {
    int value;

    ClassWithFunctionTryBlock ()
    try : value(throwingFunction()) { }
    catch (bool thrown)
    {
        functionTryBlockEntered = true;
    }
};

/**
 * Uses a function try block to catch errors thrown in an initialiser list
 */
void testFunctionTryBlocks ()
{
    try
    {
        ClassWithFunctionTryBlock();
    }
    catch (...)
    {
        assert(functionTryBlockEntered);
        return;
    }

    assert(0);
}

// Recursive template for general case
template <unsigned int N>
struct factorial
{
    enum
    {
        value = N * factorial<N - 1>::value
    };
};

// Template specialisation for base case
template <>
struct factorial<0>
{
    enum
    {
        value = 1
    };
};

/**
 * Provides computation from compile-time template specialisation
 */
void testTuringCompleteTemplateMetaProgramming ()
{
    assert(factorial<5>::value == 120);
}

int mostVexingParse(int(i));

int mostVexingParse (int i)
{
    return 5;
}

/**
 * Demonstrates that most vexing parse interprets the declaration above
 * as a function, rather than a variable initialised by another
 */
void testMostVexingParse ()
{
    const int i = 5;
    const int variableInitialisedByAnother = i;
    assert(mostVexingParse(5) == variableInitialisedByAnother);
}

namespace ADL
{
    struct UniqueClassName { };

    bool noNeedToNamespaceMe (const UniqueClassName & uniqueClassName)
    {
        return true;
    }
}

/**
 * Shows how argument-dependent lookup (ADL) can determine which function to
 * lookup based on the types of the arguments provided to it
 */
void testArgumentDependentLookup ()
{
    ADL::UniqueClassName asThisTypeIsUnique;
    assert(noNeedToNamespaceMe(asThisTypeIsUnique));
}

/**
 * BitField example struct
 * @example Layout:
 * <    p4     ><   p3   >< p2 ><p1>
 * |?|?|?|?|?|?|?|?|?|?|?|?|?|?|?|?|
 * Assigning the integer 0x4c to this structure gives the following bit pattern:
 * |0|0|0|0|0|0|0|0|0|1|0|0|1|1|0|0|
 */
struct BitField
{
    unsigned p1 : 2;
    unsigned p2 : 3;
    unsigned p3 : 5;
    unsigned p4 : 5;
};

union BitFieldUnion
{
    struct BitField bitField;
    unsigned bitInteger;
};

template <size_t A, size_t B, size_t C, size_t D>
struct TemplatedBitfield
{
    unsigned p1 : A;
    unsigned p2 : B;
    unsigned p3 : C;
    unsigned p4 : D;
};

/**
 * An example of the black art of bitfields, which can occupy less storage
 * than an integral type.  These are syntactically much easier to use as part
 * of a union.  You can even template them, too!
 */
void testBitfieldUnion ()
{
    struct BitField bitField;
    unsigned * bitFieldPointer = reinterpret_cast<unsigned *>(& bitField);
    * bitFieldPointer = 0x4c;
    const int bitFieldValue = * reinterpret_cast<unsigned *>(& bitField);

    BitFieldUnion bitFieldUnion;
    bitFieldUnion.bitInteger = 76;
    const int bitFieldUnionValue = bitFieldUnion.bitInteger;

    assert(bitFieldValue == bitFieldUnionValue);

    assert(bitField.p1 == bitFieldUnion.bitField.p1);
    assert(bitField.p2 == bitFieldUnion.bitField.p2);
    assert(bitField.p3 == bitFieldUnion.bitField.p3);
    assert(bitField.p4 == bitFieldUnion.bitField.p4);

    assert(bitField.p1 == 0);
    assert(bitField.p2 == 3);
    assert(bitField.p3 == 2);
    assert(bitField.p4 == 0);

    struct TemplatedBitfield<2, 3, 5, 5> templatedBitfield;

    unsigned * templatedBitFieldPointer =
        reinterpret_cast<unsigned *>(& templatedBitfield);

    * templatedBitFieldPointer = 0x4c;

    const int templatedBitFieldValue =
        * reinterpret_cast<unsigned *>(& templatedBitfield);

    assert(bitFieldValue == templatedBitFieldValue);
}

/**
 * Not so much a language feature, but an example of how the standard library 
 * provides some handy libraries that simplify mundane tasks.
 * Here, a stream representing an input file is read into a collection of 
 * strings in a single line, so that collection can be initialised as constant.
 * Note the use of the std namespace, as there is so much of it used!
 */
void testStreamIterators ()
{
    using namespace std;
    stringstream mockFileInput;

    mockFileInput
        << "From" << endl
        << "a" << endl
        << "file!" << endl;

     const vector<string> strings((istream_iterator<string>(mockFileInput)),
         istream_iterator<string>());

     assert(strings.size() == 3);
     assert(strings[0] == "From" && strings[1] == "a" && strings[2] == "file!");
}

/**
 * Proving that classes can be declared in a for loop, err, declaration
 */
void testUnexpectedDeclarationsInForLoop ()
{
    int count = 0;

    for (struct { int count; } loop = { 0 }; loop.count <= 5; ++loop.count)
    {
        count = loop.count;
    }

    assert(count == 5);
}

/**
 * Not a feature, more of an annoying design of map that creates a key and value
 * if that non-existent key is accessed via the brackets operator
 */
void testBewareMapBracketsOperator ()
{
    std::map<std::string, std::string> stringsToStrings;
    assert(stringsToStrings.find("didNotExist") == stringsToStrings.end());
    std::string& whyHasThisBeenFound = stringsToStrings["didNotExist"];
    assert(whyHasThisBeenFound.empty());
    assert(stringsToStrings.find("didNotExist") != stringsToStrings.end());
}

template <typename T> 
class TemplatedClassWithFriendFunction
{
    T _value;

public:
    TemplatedClassWithFriendFunction (const T value) : _value(value) { }

#ifdef THIS_WILL_FAIL
    friend void wouldCreateIdenticalDefinitions () { }
#endif
    friend T generatesDifferentTemplatedVersion
        (TemplatedClassWithFriendFunction<T> & templatedClassWithFriendFunction)
    {
        return templatedClassWithFriendFunction._value;
    } 
};

/**
 * Demonstrates how a friend function of a templated class must be defined 
 * to ensure it is generated differently for each template type used,
 * thus not violating the One Definition Rule (ODR)
 */
void testTemplatedClassWithFriendFunctionAvoidsViolatingODR ()
{
    TemplatedClassWithFriendFunction<int> integerVersion(123);
    TemplatedClassWithFriendFunction<long> longVersion(123);
    
    assert(generatesDifferentTemplatedVersion(integerVersion)
        == generatesDifferentTemplatedVersion(longVersion));
}


class Member
{
protected:
    bool _hidden;

public:
    Member () : _hidden(true) { }

    bool getHidden () const
    {
        return _hidden;
    }
};

class NormalComposition
{
    Member * _member;

public:
    NormalComposition () : _member(new Member) { }

    bool getHiddenFromMember () const
    {
        return _member->getHidden();
    }
};

struct HasAComposition : private Member
{
    HasAComposition () : Member() { }
    using Member::getHidden;

    bool accessMemberPrivates ()
    {
        return Member::getHidden();
    }
};

/**
 * Demonstrates some of the differences between normal composition and
 * private inheritance.  Among others, although a class that privately inherits
 * from another can access anything within it, only one instance can be used,
 * and it can't be forward-declared.
 */
void testCompositionViaPrivateInheritance ()
{
    assert(NormalComposition().getHiddenFromMember());
    assert(HasAComposition().getHidden());
    assert(HasAComposition().accessMemberPrivates());
}

/**
 * Simple but worthwhile comparison of the ways values can be assigned to
 * primitive types, mainly that direct initialisation isn't quite construction,
 * as it appears
 */
void testDirectInitialisation ()
{
    const int usualAssignment = 7;
    const int directInitialisation(7);
    assert(usualAssignment == directInitialisation);
}
/*
template <typename T>
class HasFriend
{
    friend T;
    bool _hidden;

public:
    HasFriend () : _hidden(true) { }

    bool getHidden () const
    {
        return _hidden;
    }
};

class FriendClass
{
public:
    void setHidden (HasFriend<FriendClass> & hasFriend, const bool value) const
    {
        hasFriend._hidden = value;
    }
};

 **
 * Friends: they're better templated?
 *
void testTemplateAsFriend ()
{
    HasFriend<FriendClass> hasFriend;
    assert(hasFriend.getHidden());

    FriendClass().setHidden(hasFriend, false);
    assert(!hasFriend.getHidden());
}
*/
class ContainsMutant 
{
    const int _value;  
    mutable bool _valueWasAccessed;

public:
    ContainsMutant (const int value)
        : _value(value), _valueWasAccessed(false) { }

    int getValue () const
    {
        _valueWasAccessed = true;
        return _value;
    }

    bool valueWasAccessed () const
    {
        return _valueWasAccessed;
    }
};

/**
 * Establishing that a mutable attribute may be modified by a method marked
 * as const, which could be useful for caching an expensive operation yet
 * not affecting the external, visible state of the object
 */
void testMutable ()
{
    const ContainsMutant containsMutant(28);
    assert(!containsMutant.valueWasAccessed());
    assert(containsMutant.getValue() == 28);
    assert(containsMutant.valueWasAccessed());
}

int changeMyArgumentDefault (const int i=10)
{
    return i;
}

/**
 * Confuse the life out of your colleagues by redeclaring a function signature
 * in a different scope, but only changing a default argument value, then
 * comparing the calls to it and the original in global scope.  I'd hide...
 */
void testChangingDefaultArguments ()
{
    int changeMyArgumentDefault (const int i=5);
    assert(changeMyArgumentDefault() == 5);
    assert(::changeMyArgumentDefault() == 10);
}

int main ()
{
    typedef void (* testFunction)();

    const std::vector<testFunction> & tests =
        CreateContainer<std::vector, testFunction>
            (& testBranchOnVariableDeclaration)
            (& testArrayIndexAccess)
            (& testKeywordOperatorTokens)
            (& testChangingScope)
            (& testPointerToMemberOperators)
            (& testMemberPointersCircumventScope)
            (& testScopeGuardTrick)
            (& testPrePostInDecrementOverloading)
            (& testFluentCommaAndBracketOverloads)
            (& testReturnOverload)
            (& testNamespaces)
            (& testTernaryAsValue)
            (& testBareURIViaGoto)
            (& testCatchAnyException)
            (& testTemplateChecksFunctionExists)
            (& testIdentityMetaFunction)
            (& testDecayArrayToPointerViaUnaryOperator)
            (& testCallSurrogateFunctions)
            (& testVoidReturn)
            (& testFindingTypeName)
            (& testFunctionTryBlocks)
            (& testTuringCompleteTemplateMetaProgramming)
            (& testMostVexingParse)
            (& testArgumentDependentLookup)
            (& testBitfieldUnion)
            (& testStreamIterators)
            (& testUnexpectedDeclarationsInForLoop)
            (& testBewareMapBracketsOperator)
            (& testTemplatedClassWithFriendFunctionAvoidsViolatingODR)
            (& testCompositionViaPrivateInheritance)
            (& testDirectInitialisation)
          //(& testTemplateAsFriend)
            (& testMutable)
            (& testChangingDefaultArguments)
        .get();

    const size_t & numberOfTests = tests.size();

    for (size_t i = 0; i < numberOfTests; ++i)
    {
        tests[i]();
    }

    std::cout << numberOfTests << " tests passed successfully!" << std::endl;
    return EXIT_SUCCESS;
}
