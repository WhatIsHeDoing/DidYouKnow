=head1 NAME

DidYouKnow::Regex

=head1 DESCRIPTION

Unit testing Perl regular expressions.
https://perldoc.perl.org/perlre

=cut

package DidYouKnow::Regex;

use strict;
use warnings;

use parent qw(Test::Class);
use Test::More;

sub testRegexMultiLineMatch : Test(no_plan) {
    my $string = "beats the usual\nhello\nworld\ntest";
    my $regex  = m/^hello(.*)test/sm;
    ok $string =~ $regex;
}

1;
