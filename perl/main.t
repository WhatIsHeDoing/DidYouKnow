#! /usr/bin/perl
=head1 NAME

main

=head1 DESCRIPTION

Runs all the test methods in the DidYouKnow package
http://perldoc.perl.org/Test/More.html

=cut

use DidYouKnow::OneLiners;
use DidYouKnow::Regex;
Test::Class->runtests;
