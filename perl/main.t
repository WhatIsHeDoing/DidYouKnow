#! /usr/bin/perl
=head1 NAME

main

=head1 DESCRIPTION

Runs all the test methods in the DidYouKnow package

=cut

use DidYouKnow::OneLiners;
Test::Class->runtests;
