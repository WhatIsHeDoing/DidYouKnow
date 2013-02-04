""" demonstrating some great Python features via unit tests """
import unittest

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
    