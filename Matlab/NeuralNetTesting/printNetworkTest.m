function printNetworkTest(ysRaw, Ymax, Ythresh, delay, printAll)
%printNetworkTest displays test results from the testNetwork function.
%
% Inputs:
%  ysRaw    = Raw ground truth values for comparison
%  Ymax     = Index of maximum value as determined by the network
%  Ythresh  = Y values that were above the given threshold value
%  delay    = Number of delay steps in the network
%  printAll = Toggle to only print lines where the network misclassified a
%               gesture.

[m, n] = size(Ythresh);

fprintf('Number: True| Max | Above Threshold\n');
for i = 1:m
    if (printAll || ysRaw(i + delay) ~= Ymax(i))
        fprintf(['%6d: %3d | %3d | ' repmat('%d ', 1, n)], ...
            i, ysRaw(i + delay), Ymax(i), Ythresh(i, :));
        fprintf('\n');
    end;
end;
fprintf('Number: True| Max | Above Threshold\n');
error = sum(ysRaw(delay + 1:end) ~= Ymax(:)) / m;
fprintf('Error rate: %6f\n', error);

end