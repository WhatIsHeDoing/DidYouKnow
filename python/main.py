""" demonstrating some great Python features via unit tests """
from functools import partial
import operator
import os
import tempfile
import unittest

class testRandomFeatures(unittest.TestCase):
    """ testing random language features """

    def testMultipleAssignment(self):
        """ tests the shorthand syntax of assigning values to multiple variables """
        x, y = 1, 2
        self.assertEqual(1, x)
        self.assertEqual(2, y)

        # from lists indices
        x, y, *z = [1, 2, 3, 4]
        self.assertEqual(1, x)
        self.assertEqual(2, y)
        self.assertEqual([3, 4], z, "all other values!")

        # in-place value swapping"
        y, x = x, y
        self.assertEqual(2, x)
        self.assertEqual(1, y)

        def returns_multiple_values():
            return (1, 2)

        x, y = returns_multiple_values()
        self.assertEqual(1, x)
        self.assertEqual(2, y)

    def testForElse(self):
        """ tests that an "else" statement is entered if a for loop is not exited """
        for i in range(0, 5):
            if i > 5:
                break
        else:
            return

        self.fail()

    def testWith(self):
        """ tests unmanaged resources are closed via the "with" keyword """
        filename = ""

        with tempfile.NamedTemporaryFile() as file_handle:
            filename = file_handle.name
            self.assertTrue(os.path.exists(filename))

        self.assertFalse(os.path.exists(filename))

    def testChainedConditions(self):
        """ tests the much-loved feature of chaining conditions """
        self.assertTrue(1 < 2 < 3)
        self.assertTrue(1 < 3 > 2)

    def testStringTemplating(self):
        """ shows the simple yet powerful ability to template strings """
        from string import Template
        xml_template = Template('<${tag}>${content}</${tag}>')
        actual = xml_template.substitute(tag='h1', content='Hello, world!')
        expected = "<h1>Hello, world!</h1>"
        self.assertEqual(expected, actual)

    def testDecorators(self):
        """
        tests the very useful ability to decorate functions with other functions
        classes can also be used
        """
        def paragraph_me(func):
            def _paragraph_me(args):
                return "<p>" + func(args) + "</p>"
            return _paragraph_me

        @paragraph_me
        def to_paragraph(value):
            return value

        self.assertEqual(to_paragraph("Hello, world!"), "<p>Hello, world!</p>")

    def testBewareMutableFunctionArguments(self):
        """
        default function argument values are stored in a tuple as part of the
        function, so use a sentinel value to denote "not given" and replace with
        the mutable as a default
        """
        def mutates_default_arg(arg=[]):
            arg.append(1)
            return arg

        self.assertEqual(len(mutates_default_arg.__defaults__), 1)
        self.assertEqual(mutates_default_arg(), [1])
        self.assertEqual(mutates_default_arg.__defaults__[0], [1])
        self.assertEqual(mutates_default_arg(), [1, 1])
        self.assertEqual(mutates_default_arg.__defaults__[0], [1, 1])

        def does_not_mutate_default_arg(arg=None):
            arg = [] if arg is None else arg
            arg.append(1)
            return arg

        self.assertEqual(len(does_not_mutate_default_arg.__defaults__), 1)
        self.assertEqual(does_not_mutate_default_arg(), [1])
        self.assertEqual(does_not_mutate_default_arg.__defaults__[0], None)
        self.assertEqual(does_not_mutate_default_arg(), [1])
        self.assertEqual(does_not_mutate_default_arg.__defaults__[0], None)

    def testInterestingOperatorUsage(self):
        """ why restrict yourself to the usual use of operators? """
        self.assertEqual("to" * 2, "toto")
        self.assertEqual(2 * "to", "toto")
        self.assertEqual([1, 2] * 2, [1, 2, 1, 2])
        self.assertEqual("Keep me!" * True, "Keep me!")
        self.assertEqual("Keep me!" * False, "")
        self.assertEqual([1, 2] + [3, 4], [1, 2, 3, 4])

    def testMembership(self):
        """ using the "in" keyword to test memberships """
        self.assertTrue("b" in "abc")
        self.assertFalse("b" in "efg")
        self.assertTrue(2 in [1, 2, 3])
        self.assertFalse(2 in [4, 5, 6])

    def testZip(self):
        """ showing how lists can be iterated together, and how to transpose them """
        for zipped in zip(["one"], [2]):
            self.assertEqual(zipped, ("one", 2))

        transpose_me = [(1, 2), (3, 4), (5, 6)]
        self.assertEqual(list(zip(*transpose_me)), [(1, 3, 5), (2, 4, 6)])

    def testEllipsisOperator(self):
        """ demonstrates how the ellipsis operator can be used """
        class TestEllipsis(object):
            def __getitem__(self, item):
                if item is Ellipsis:
                    return "Returning all items"

                return "return %r items" % item

        test_ellipsis = TestEllipsis()
        self.assertEqual(test_ellipsis[2], "return 2 items")
        self.assertEqual(test_ellipsis[...], "Returning all items")

    def testFunctools(self):
        """ testing the binding of arguments to functions for late evaluation """
        bound_func = partial(range, 0, 4)
        self.assertEqual(list(bound_func()), [0, 1, 2, 3])
        self.assertEqual(list(bound_func(2)), [0, 2])

    def functionsAsFirstClassObjects(self):
        """ passing functions around as they are first-class objects """
        called_back = False

        def callback():
            called_back = True

        def use_callback(func):
            func()

        use_callback(callback)
        self.assertTrue(called_back)

class testExceptions(unittest.TestCase):
    """ showcasing the interesting additions to exception handling """

    def testTryExceptElseFinally(self):
        """ tests the flow of an exception-handling block """
        try:
            1 / 0
        except ZeroDivisionError:
            pass
        else:
            self.fail()
        finally:
            return

        self.fail()

    def testReRaiseException(self):
        """ shows how an exception can be re-raised to preserve its original traceback """
        try:
            try:
                1 / 0
            except ZeroDivisionError as e:
                raise
        except ZeroDivisionError as e:
            return

        self.fail()

class testIterating(unittest.TestCase):
    """ demonstrates the support for different uses of iterators """

    def testOperatorSorting(self):
        """ demonstrates sorting objects on any of their keys without custom functions """
        class Custom:
            def __init__(self, value):
                self.id = value

        customs = [Custom(5), Custom(3)]
        customs.sort(key=operator.attrgetter('id'))
        self.assertEqual(3, customs[0].id)

    def testGenerators(self):
        """ shows the awesome ability to yield values as they are needed """
        def generate_to(max_value):
            for i in range(0, max_value):
                yield i

        for i in generate_to(3):
            self.assertEqual(i, 0)
            return

    def testSendingValuesToGenerators(self):
        """ demonstrates how values can be received by generators """
        def generate_value(value):
            while True:
                received_value = (yield value)

                if received_value is not None:
                    value = received_value

        generator = generate_value(5)
        self.assertEqual(next(generator), 5)
        self.assertEqual(next(generator), 5)
        self.assertEqual(generator.send(7), 7)
        self.assertEqual(next(generator), 7)

    def arraySlicing(self):
        """ slicing arrays in many different ways """
        values = [1, 2, 3, 4, 5]
        self.assertEqual(values[2:], [3, 4, 5], "from an index onwards")

        self.assertEqual(values[-1], [5],
            "using a negative index to access indices from end of the array")

        self.assertEqual(values[:2], [1, 2], "up to an index")
        self.assertEqual(values[2:4], [3, 4], "between indices")
        self.assertEqual(values[::2], [2, 4], "using a custom interval")
        self.assertEqual(values[::-1], [5, 4, 3, 2, 1], "reversing")

class testSets(unittest.TestCase):
    """ shows the support for sets and comparisons of them """

    def setUp(self):
        """ initialise the sets used throughout the tests """
        self.one = set(' abcde ')
        self.two = set('  b d f')

    def testDifference(self):
        """ shows the members missing from the second set that are in the first """
        self.assertEqual(self.one - self.two, self.one.difference(self.two))
        difference = list((self.one - self.two))
        difference.sort()
        self.assertEqual(difference, ["a", "c", "e"])

    def testUnion(self):
        """ tests the union of two sets """
        self.assertEqual(self.one | self.two, self.one.union(self.two))
        union = list((self.one | self.two))
        union.sort()
        self.assertEqual(union, [" ", "a", "b", "c", "d", "e", "f"])

    def testIntersection(self):
        """ finds the common members of two sets """
        self.assertEqual(self.one & self.two, self.one.intersection(self.two))
        intersection = list((self.one & self.two))
        intersection.sort()
        self.assertEqual(intersection, [" ", "b", "d"])

    def testSymmetricDifference(self):
        """ shows all the values that are members of one set and not the other """
        self.assertEqual(self.one ^ self.two, self.one.symmetric_difference(self.two))
        symmetric_difference = list((self.one ^ self.two))
        symmetric_difference.sort()
        self.assertEqual(symmetric_difference, ["a", "c", "e", "f"])

class testListComprehension(unittest.TestCase):
    """ why loop when you can comprehend?! """

    def testProjection(self):
        """ tests projecting the items from one list to another """
        expected = [2, 4, 6]
        actual = [n * 2 for n in [1, 2, 3]]
        self.assertEqual(expected, actual)

    def testNested(self):
        """ tests nested projection to transpose nested lists """
        matrix = [
            [1, 2, 3],
            [4, 5, 6],
            [7, 8, 9]
        ]

        expected = [
            [1, 4, 7],
            [2, 5, 8],
            [3, 6, 9]
        ]

        actual = [[row[i] for row in matrix] for i in range(len(matrix))]
        self.assertEqual(expected, actual)

    def testRestriction(self):
        """ tests restricting a list and finding the odd numbers """
        expected = [1, 3]
        actual = [n for n in [1, 2, 3] if n % 2 == 1]
        self.assertEqual(expected, actual)

class testUnpacking(unittest.TestCase):
    """ tests the neat feature of variable unpacking """

    @staticmethod
    def rectangle_area(height=1, width=1):
        """ simple function that multiplies named parameters """
        return height * width

    def testArray(self):
        """
        tests unpacking an array so that each
        element is used as an argument to a function
        """
        rectangle = [2, 4]
        actual = self.rectangle_area(*rectangle)
        expected = rectangle[0] * rectangle[1]
        self.assertEqual(actual, expected)

    def testDictionary(self):
        """
        tests unpacking a dictionary so that each
        key-value pair is used as a named argument to a function
        """
        rectangle = {
            "height": 2,
            "width": 4
        }

        actual = self.rectangle_area(**rectangle)
        expected = rectangle["height"] * rectangle["width"]
        self.assertEqual(actual, expected)

    def testTuple(self):
        """
        tests unpacking a tuple into individual variables
        and an array of unknown length
        """
        my_tuple = (1, 2, 3, 4)
        first, second, *others = my_tuple
        self.assertEqual(first, my_tuple[0])
        self.assertEqual(second, my_tuple[1])
        self.assertEqual(others, list(my_tuple[2:4]))

if __name__ == "__main__":
    unittest.main()
