function [Y, Yzeroed, Ythresh, Ymax] = testNetwork(networkFunction, xs, ...
    delay, thresholds)
%TESTNETWORK gets various output data from a given test set and network.
% Inputs:
%  networkFunction  = Function handle for the trained network.
%  xs               = Raw input.
%  delay            = Number of delay steps in the network.
%  thresholds       = Threshold values to test.
% 
% Outputs:
%  Y        = raw outputs.
%  Yzeroed  = outputs with below-threshold values zeroed.
%  Ythresh  = logical vectors of outputs with above-threshold values == 1.
%  Ymax     = index of highest value in output matrix.

[Ybase, ~] = runOutputFunction(xs, networkFunction, delay);
Y = cell2mat(Ybase);
Y = Y';

[m, ~] = size(Y);
threshRep = repmat(thresholds, m, 1);

Ythresh = Y > threshRep;
Yzeroed = Y .* Ythresh;
[~, Ymax] = max(Y, [], 2);

end