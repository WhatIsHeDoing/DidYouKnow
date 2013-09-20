#include <algorithm>
#include <cassert>
#include <cstdlib>
#include <iostream>
#include <map>
#include <stdexcept>
#include <string>
#include <vector>

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

void testArrayIndexAccess ()
{
    int array[3];
    array[0] = 0;
    array[1] = 1;
    array[2] = 2;

    assert(array[1] == 1);
    assert(*(array + 1) == 1);
    assert((*(1 + array)) == 1);
    assert(1[array] == 1);
}

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

void testScopeGuardTrick ()
{
    const std::string & value(letMeKeepMyReturnValue());
    assert(value == "value");
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

void testCommaAndBracketOverloads ()
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

void testReturnOverload ()
{
    ReturnOverload returnOverload;
    const int & i(returnOverload.get());
    assert(i == 5);

    const ReturnOverload constReturnOverload;
    const std::string & s(constReturnOverload.get());
    assert(s == "hello");
}

struct TypedefScopedToClass
{
    typedef std::vector<std::string> strings;
};

void testTypedefScopedToClass ()
{
    std::vector<std::string> v1;
    v1.push_back("hello");

    TypedefScopedToClass::strings v2;
    v2.push_back("hello");

    assert(v1 == v2);
}

bool ternaryTrue ()
{
    return true;
}

bool ternaryFalse ()
{
  return false;
}

void testTernaryAsValue ()
{
    int postiveCount(0);
    int negativeCount(0);
    (5 < 0 ? negativeCount : postiveCount)++;
    assert(postiveCount == 1);
    assert(negativeCount == 0);

    assert((1 ? ternaryTrue : ternaryFalse)());
}

void testBareURIUsingGoto ()
{
    goto http;
    assert(0);
    return;

    http://test.com
    assert(1);
}

namespace HeadNamespace {
    namespace SubNamespace {
        bool exists ()
        {
            return true;
        }
    }
}

void testNamespaceAlias ()
{
    namespace alias = HeadNamespace::SubNamespace;
    assert(alias::exists);
}

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

void testIdentityMetaFunction ()
{
    ID<bool()>::type * IDFunc = & calledViaIdentityDefinition;
    assert(IDFunc());
}

int main ()
{
    typedef void (* testFunc)();

    const std::vector<testFunc> tests(
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
            (& testCommaAndBracketOverloads)
            (& testReturnOverload)
            (& testTypedefScopedToClass)
            (& testTernaryAsValue)
            (& testBareURIUsingGoto)
            (& testNamespaceAlias)
            (& testCatchAnyException)
            (& testTemplateChecksFunctionExists)
            (& testIdentityMetaFunction)
        .get()
    );

    const int & numOfTests(tests.size());

    for (int i = 0; i < numOfTests; ++i)
    {
        tests[i]();
    }

    std::cout << numOfTests << " tests passed successfully!" << std::endl;
    return EXIT_SUCCESS;
}
