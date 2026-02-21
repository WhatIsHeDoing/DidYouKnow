#!/usr/bin/env perl

=head1 NAME

main

=head1 DESCRIPTION

Runs all the test methods in the DidYouKnow package
https://perldoc.perl.org/Test/Class

=cut

use strict;
use warnings;

use DidYouKnow::OneLiners;
use DidYouKnow::Regex;

Test::Class->runtests;
