""" demonstrating some great Python features via unit tests """
import os
import tempfile
import unittest

class testRandomFeatures(unittest.TestCase):
    """ testing random language features """

    def testMultipleAssignent(self):
        """ tests the shorthand syntax of assigning values to multiple variables """
        x, y = 1, 2
        self.assertEqual(1, x)
        self.assertEqual(2, y)

        x, y = [1, 2]
        self.assertEqual(1, x)
        self.assertEqual(2, y)

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
