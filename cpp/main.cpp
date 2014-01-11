#include <algorithm>
#include <cassert>
#include <cstdlib>
#include <iostream>
#include <map>
#include <stdexcept>
#include <string>
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
    array[2] = 2;

    assert(array[2] == *(array + 2) == (*(1 + array)) == 1[array] == 1);
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

class ContainsProtectedValue
{
public:
    ContainsProtectedValue (const int foo) : foo(foo) { }

protected:
    const int foo;
};

class PromotesProtectedValue : public ContainsProtectedValue
{
public:
    PromotesProtectedValue (int foo) : ContainsProtectedValue(foo) { }
    using ContainsProtectedValue::foo;
};

/**
 * Demonstrates how to alter the scope of class members in their derived classes
 */
void testChangingScope ()
{
    assert(PromotesProtectedValue(5).foo == 5);
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

struct HasStaticMethod {
    static bool isCalled ()
    {
        return true;
    }
};

/*
 * Simply shows the syntax required to call the static method of a class
 */
void testStaticInstanceMethodCalls ()
{
    assert(HasStaticMethod::isCalled() == HasStaticMethod().isCalled());
}

struct PointToUs {
    int value;

    bool method () const
    {
        return true;
    }
};

int PointToUs::*valuePointer = &PointToUs::value;
bool (PointToUs::*methodPointer)() const = &PointToUs::method;

/**
 * Demonstrates the syntax required for pointers to function members,
 * which - when required - may be a pain to remember quickly
 */
void testPointerToMemberOperators ()
{
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
class VectorBuilder
{
private:
    V< T, std::allocator<T> > _container;

public:
    VectorBuilder & addValue (const T & value)
    {
        _container.push_back(value);
        return * this;
    }

    VectorBuilder () { }

    VectorBuilder (const T & value)
    {
        addValue(value);
    }

    VectorBuilder & operator, (const T & value)
    {
        return addValue(value);
    }

    VectorBuilder & operator() (const T & value)
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
    assert((VectorBuilder<std::vector, int>(0), 1).get() == integers);

    std::vector<std::string> strings;
    strings.push_back("hello");
    strings.push_back("world");
    assert((VectorBuilder<std::vector, std::string>("hello")("world").get()) == strings);
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
        bool HasThisFunction ()
        {
            return true;
        }
    }
}

struct ThisClass
{
    static bool HasThisFunction ()
    {
        return true;
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
    assert(ThisClass::HasThisFunction());
    assert(ThisClass::HasThisTypedef(true));

    namespace alias = ThisNamespace::SubNamespace;
    assert(alias::HasThisFunction());
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
void testBareURIUsingGoto ()
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

template<typename Func1, typename Func2>
class CallsSurrogates {
    Func1 * _func1;
    Func2 * _func2;

public:
    CallsSurrogates(Func1 * func1, Func2 * func2)
        : _func1(func1), _func2(func2) { }

    operator Func1 * ()
    {
        return _func1;
    }

    operator Func2 * ()
    {
        return _func2;
    }
};

std::string intFunc (int i)
{
    return "integer passed";
}

std::string longFunc (long i)
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
        callsSurrogates(intFunc, longFunc);

    assert(callsSurrogates(5) == "integer passed");
    assert(callsSurrogates(5L) == "long passed");
}

void voidReturn ()
{
    return (void)"say what?!";
}

/**
 * A demonstration of how void casting lets you place
 * pretty much anything you like after the cast
 */
void testVoidReturn ()
{
    voidReturn();
}

int main ()
{
    typedef void (* testFunc)();

    const std::vector<testFunc> & tests =
        VectorBuilder<std::vector, testFunc>
            (& testBranchOnVariableDeclaration)
            (& testArrayIndexAccess)
            (& testKeywordOperatorTokens)
            (& testChangingScope)
            (& testRedefiningKeywords)
            (& testStaticInstanceMethodCalls)
            (& testPointerToMemberOperators)
            (& testScopeGuardTrick)
            (& testPrePostInDecrementOverloading)
            (& testFluentCommaAndBracketOverloads)
            (& testReturnOverload)
            (& testNamespaces)
            (& testTernaryAsValue)
            (& testBareURIUsingGoto)
            (& testCatchAnyException)
            (& testTemplateChecksFunctionExists)
            (& testIdentityMetaFunction)
            (& testDecayArrayToPointerViaUnaryOperator)
            (& testCallSurrogateFunctions)
            (& testVoidReturn)
        .get();

    const size_t & numOfTests = tests.size();

    for (unsigned int i = 0; i < numOfTests; ++i)
    {
        tests[i]();
    }

    std::cout << numOfTests << " tests passed successfully!" << std::endl;
    return EXIT_SUCCESS;
}
