function [Y, Xf] = runOutputFunction(xs, fn, delay)

Xi = cell(1, delay);
Xi{1} = xs(1, :)';
Xi{2} = xs(2, :)';
X = cell(1, size(xs, 1) - 2);

for i = 1:size(X, 2)
    X{i} = xs(i + 2, :)';
end;

[Y, Xf] = fn(X, Xi);

end