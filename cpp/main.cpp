#include <cassert>
#include <cstdlib>
#include <iostream>
#include <map>
#include <string>

void testMapAssignLookupToValue ()
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

// START OBSCURE
// http://madebyevan.com/obscure-cpp-features/
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
// END OBSCURE

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

int main ()
{
  testMapAssignLookupToValue();
  testArrayIndexAccess();
  testKeywordOperators();
  testChangingScope();
  return EXIT_SUCCESS;
}
