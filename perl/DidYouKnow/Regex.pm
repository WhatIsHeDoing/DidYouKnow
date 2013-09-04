=head1 NAME

OneLiners

=head1 DESCRIPTION

Unit testing Perl regular expressions
http://perldoc.perl.org/perlre.html

=cut

package Regex;
use base qw(Test::Class);
use Test::More;

sub testRegexMultiLineMatch : Test(no_plan)
{
	my $string = "beats the usual\nhello\nworld\ntest";
	my $regex = m/^hello(.*)test/sm;
	ok $string =~ $regex;
};

return 1;
