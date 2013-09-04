=head1 NAME

OneLiners

=head1 DESCRIPTION

Unit testing Perl one-liners with Perl!
This way, we can be almost certain that the tests will run if Perl is installed
The one-liners are run with backticks (`) rather than system() for clarity

=cut

package OneLiners;
use base qw(Test::Class);
use File::Slurp;
use Test::More;

sub setUp : Test(setup)
{
	members->{tempFileName} = "test.js";
	members->{tempFileBackupName} = "test.js.bak";
};

sub testOneLinerInlineReplace : Test(no_plan)
{
	open my $file, ">", members->{tempFileName} or fail $!;

	print $file "if (a == b) alert('equality fail');
if (c === d) alert('equality fail');\n";

	close $file or fail($!);

	`perl -pi.bak -e "s/alert/console\.info/g" test.js`;

	my $expected = "if (a == b) console.info('equality fail');
if (c === d) console.info('equality fail');\n";

	is read_file(members->{tempFileName}), $expected;
}

sub testOneLinerFiltering : Test(no_plan)
{
	open my $file, ">", members->{tempFileName} or fail $!;
	my $keptLine = "Keep me\n";
	my $ignoredLine = "Ignore me\n";
	print $file $keptLine;
	print $file $ignoredLine;
	close $file or fail($!);

	`perl -ni.bak -e "print unless /^Ignore/" test.js`;

	is read_file(members->{tempFileName}), $keptLine;
};

sub tearDown : Test(teardown)
{
	unlink(members->{tempFileName}) or fail $!;
	unlink(members->{tempFileBackupName}) or fail $!;
};

return 1;
