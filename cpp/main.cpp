#include <cassert>
#include <cstdlib>
#include <iostream>
#include <map>
#include <stdexcept>
#include <string>

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
	bool success ()
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

  bool method ()
  {
  	return true;
  }
};

int PointToUs::*valuePointer = &PointToUs::value;
bool (PointToUs::*methodPointer)() = &PointToUs::method;

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

int main ()
{
  testBranchOnVariableDeclaration();
  testArrayIndexAccess();
  testKeywordOperatorTokens();
  testChangingScope();
  testRedefiningKeywords();
  testStaticInstanceMethodCalls();
  testPointerToMemberOperators();
  return EXIT_SUCCESS;
}
