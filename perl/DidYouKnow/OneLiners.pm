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
	members->{tempFileName} = "test.txt";
	members->{tempFileBackupName} = "test.txt.bak";
};

sub testOneLinerFiltering : Test(no_plan)
{
	open my $file, ">", members->{tempFileName} or fail $!;
	my $keptLine = "Keep me\n";
	my $ignoredLine = "Ignore me\n";
	print $file $keptLine;
	print $file $ignoredLine;
	close $file or fail($!);

	`perl -ni.bak -e "print unless /^Ignore/" test.txt`;

	is $keptLine, read_file(members->{tempFileName});
};

sub tearDown : Test(teardown)
{
	unlink(members->{tempFileName}) or fail $!;
	unlink(members->{tempFileBackupName}) or fail $!;
};

return 1;
