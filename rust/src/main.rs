fn main() {}

#[cfg(test)]
mod tests {
    use std::mem;

    /// The turbofish syntax `::<>` lets you explicitly specify generic type
    /// parameters on functions, which is sometimes needed when the compiler
    /// cannot infer the type from context alone.
    #[test]
    fn turbofish_syntax() {
        let parsed = "42".parse::<i32>().unwrap();
        assert_eq!(parsed, 42);

        let collected = [1, 2, 3].iter().copied().collect::<Vec<_>>();
        assert_eq!(collected, vec![1, 2, 3]);
    }

    /// Labelled blocks can return values, not just labelled loops.
    /// This provides a way to "break out" of a block early with a value,
    /// similar to an early return but scoped to a block.
    #[test]
    fn labelled_block_values() {
        let result = 'block: {
            if true {
                break 'block 42;
            }
            0
        };

        assert_eq!(result, 42);
    }

    /// The `@` binding lets you bind a value to a variable while simultaneously
    /// testing it against a pattern, avoiding the need to destructure and then reconstruct.
    #[test]
    fn at_bindings_in_patterns() {
        let value = 5;

        let description = match value {
            n @ 1..=5 => format!("{n} is between 1 and 5"),
            n @ 6..=10 => format!("{n} is between 6 and 10"),
            n => format!("{n} is out of range"),
        };

        assert_eq!(description, "5 is between 1 and 5");
    }

    /// Rust allows shadowing a variable with a new binding of the same name
    /// but a completely different type, which is particularly useful for
    /// transforming a value through a series of steps.
    #[test]
    fn shadowing_changes_type() {
        let value = "42";
        assert_eq!(value, "42");

        let value = value.parse::<i32>().unwrap();
        assert_eq!(value, 42);

        let value = value > 0;
        assert!(value);
    }

    /// Multiple patterns can be combined in a single match arm using `|`,
    /// reducing repetition when several cases share the same behaviour.
    #[test]
    fn or_patterns_in_match() {
        let classify = |n: i32| match n {
            0 => "zero",
            1 | 2 | 3 => "small",
            4..=9 => "medium",
            _ => "large",
        };

        assert_eq!(classify(2), "small");
        assert_eq!(classify(7), "medium");
        assert_eq!(classify(100), "large");
    }

    /// Arrays can be initialised with a repeated value using `[value; count]`
    /// syntax, which is far more concise than listing each element.
    #[test]
    fn array_repetition_syntax() {
        let zeroes = [0u8; 5];
        assert_eq!(zeroes, [0, 0, 0, 0, 0]);

        let grid = [[1; 3]; 2];
        assert_eq!(grid, [[1, 1, 1], [1, 1, 1]]);
    }

    /// Inclusive ranges with `..=` include both endpoints,
    /// unlike the exclusive `..` range which excludes the upper bound.
    #[test]
    fn inclusive_ranges() {
        let exclusive: Vec<i32> = (1..5).collect();
        let inclusive: Vec<i32> = (1..=5).collect();

        assert_eq!(exclusive, vec![1, 2, 3, 4]);
        assert_eq!(inclusive, vec![1, 2, 3, 4, 5]);
    }

    /// The `ref` keyword in patterns creates a reference to a value instead of moving it,
    /// which is the opposite of `&` in patterns that destructures a reference.
    #[test]
    fn ref_keyword_in_patterns() {
        let value = String::from("hello");

        let ref borrowed = value;
        assert_eq!(borrowed, &value);

        // `value` is still usable because it was borrowed, not moved.
        assert_eq!(value, "hello");
    }

    /// Raw identifiers allow you to use reserved keywords as names by
    /// prefixing them with `r#`, which is useful for FFI or when working
    /// with code generated from other languages.
    #[test]
    fn raw_identifiers() {
        fn r#match(value: i32) -> &'static str {
            if value > 0 {
                "positive"
            } else {
                "non-positive"
            }
        }

        fn r#type() -> &'static str {
            "I'm a function named after a keyword!"
        }

        assert_eq!(r#match(5), "positive");
        assert!(r#type().contains("keyword"));
    }

    /// Rust automatically coerces `&String` to `&str`, `&Vec<T>` to `&[T]`,
    /// and `&Box<T>` to `&T` through the `Deref` trait, meaning you rarely need to convert explicitly.
    #[test]
    fn deref_coercion() {
        fn takes_str(s: &str) -> usize {
            s.len()
        }

        let owned = String::from("hello");
        // &String automatically coerces to &str
        assert_eq!(takes_str(&owned), 5);

        fn takes_slice(s: &[i32]) -> usize {
            s.len()
        }

        let owned = vec![1, 2, 3];
        // &Vec<i32> automatically coerces to &[i32]
        assert_eq!(takes_slice(&owned), 3);
    }

    /// `mem::replace` swaps a value out of a mutable reference, returning
    /// the old value while leaving a new one in its place, which is
    /// invaluable when you need to move out of a borrowed context.
    #[test]
    fn mem_replace_swap() {
        let mut value = String::from("old");
        let previous = mem::replace(&mut value, String::from("new"));

        assert_eq!(previous, "old");
        assert_eq!(value, "new");

        let mut a = 1;
        let mut b = 2;
        mem::swap(&mut a, &mut b);

        assert_eq!(a, 2);
        assert_eq!(b, 1);
    }

    /// Closures capture variables from their environment either by borrowing or by moving,
    /// and the `move` keyword forces ownership transfer into the closure.
    #[test]
    fn closure_capture_semantics() {
        let mut count = 0;

        // Closure borrows `count` mutably.
        let mut increment = || {
            count += 1;
        };
        increment();
        increment();
        drop(increment);

        assert_eq!(count, 2);

        let name = String::from("world");

        // `move` transfers ownership of `name` into the closure.
        let greeting = move || format!("hello, {name}");
        assert_eq!(greeting(), "hello, world");

        // `name` is no longer accessible here because it was moved.
        // let _ = name; // would not compile
    }

    /// `impl Trait` in return position lets you return a type without naming it,
    /// which is the only way to return closures and iterator adaptor chains from functions.
    #[test]
    fn impl_trait_return_position() {
        fn make_adder(x: i32) -> impl Fn(i32) -> i32 {
            move |y| x + y
        }

        let add_five = make_adder(5);
        assert_eq!(add_five(3), 8);

        fn even_numbers(limit: i32) -> impl Iterator<Item = i32> {
            (0..limit).filter(|n| n % 2 == 0)
        }

        let evens: Vec<_> = even_numbers(10).collect();
        assert_eq!(evens, vec![0, 2, 4, 6, 8]);
    }

    /// Unlike most languages, `if` is an expression in Rust, meaning it
    /// returns a value, removing the need for a ternary operator.
    #[test]
    fn if_as_expression() {
        let x = 5;
        let description = if x > 0 { "positive" } else { "non-positive" };
        assert_eq!(description, "positive");

        // Nested if-else chains also work as expressions
        let category = if x < 0 {
            "negative"
        } else if x == 0 {
            "zero"
        } else {
            "positive"
        };

        assert_eq!(category, "positive");
    }

    /// Slice patterns let you destructure arrays and slices by matching on their elements,
    /// including using `..` to match a variable number of elements in the middle.
    #[test]
    fn slice_patterns() {
        let values = [1, 2, 3, 4, 5];

        let [first, .., last] = values;
        assert_eq!(first, 1);
        assert_eq!(last, 5);

        let [_, second, rest @ ..] = values;
        assert_eq!(second, 2);
        assert_eq!(rest, [3, 4, 5]);
    }

    /// Match guards add extra conditions to match arms, letting you
    /// express complex logic that cannot be captured by patterns alone.
    #[test]
    fn match_guards() {
        let pair = (2, -2);

        let description = match pair {
            (x, y) if x == y => "equal",
            (x, y) if x + y == 0 => "opposites",
            _ => "other",
        };

        assert_eq!(description, "opposites");
    }

    /// Destructuring assignments let you unpack structs, tuples, and enums
    /// directly in `let` bindings, function parameters, and even loop variables.
    #[test]
    fn destructuring() {
        struct Point {
            x: i32,
            y: i32,
        }

        let Point { x, y } = Point { x: 3, y: 7 };
        assert_eq!(x, 3);
        assert_eq!(y, 7);

        // Renaming during destructure.
        let Point {
            x: horizontal,
            y: vertical,
        } = Point { x: 1, y: 2 };

        assert_eq!(horizontal, 1);
        assert_eq!(vertical, 2);

        // Tuple destructuring in a for loop.
        let pairs = vec![(1, "one"), (2, "two")];

        for (number, name) in &pairs {
            assert!((*number == 1 && *name == "one") || (*number == 2 && *name == "two"));
        }
    }
}
