=head1 NAME

DidYouKnow::OneLiners

=head1 DESCRIPTION

Unit testing Perl one-liners with Perl!
This way, we can be almost certain that the tests will run if Perl is installed.
The one-liners are run with backticks (`) rather than system() for clarity.

=cut

package DidYouKnow::OneLiners;

use strict;
use warnings;

use parent qw(Test::Class);
use File::Slurp;
use Test::More;

sub setUp : Test(setup) {
    my $self = shift;
    $self->{tempFileName}       = 'test.js';
    $self->{tempFileBackupName} = 'test.js.bak';
}

sub testOneLinerInlineReplace : Test(no_plan) {
    my $self = shift;

    open my $file, '>', $self->{tempFileName} or fail $!;
    print $file "if (a == b) alert('equality fail');\n";
    print $file "if (c === d) alert('equality fail');\n";
    close $file or fail($!);

    `perl -pi.bak -e "s/alert/console\.info/g" test.js`;

    my $expected = "if (a == b) console.info('equality fail');\n"
        . "if (c === d) console.info('equality fail');\n";

    is read_file( $self->{tempFileName} ), $expected;
}

sub testOneLinerFiltering : Test(no_plan) {
    my $self = shift;

    open my $file, '>', $self->{tempFileName} or fail $!;
    my $keptLine    = "Keep me\n";
    my $ignoredLine = "Ignore me\n";
    print $file $keptLine;
    print $file $ignoredLine;
    close $file or fail($!);

    `perl -ni.bak -e "print unless /^Ignore/" test.js`;

    is read_file( $self->{tempFileName} ), $keptLine;
}

sub tearDown : Test(teardown) {
    my $self = shift;
    unlink( $self->{tempFileName} )       or fail $!;
    unlink( $self->{tempFileBackupName} ) or fail $!;
}

1;
