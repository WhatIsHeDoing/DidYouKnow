""" demonstrating some great Python features via unit tests """
import unittest

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
    