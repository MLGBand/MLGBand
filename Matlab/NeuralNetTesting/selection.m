xs = [ ...
        LinearAccelerationX, LinearAccelerationY, LinearAccelerationZ, ...
    EulerP, EulerR, ...
    GravityX, GravityY, GravityZ, ...
%    X_MeanAdjIntegral_0995, ...
%    Y_MeanAdjIntegral_0995, ...
%    Z_MeanAdjIntegral_0995, ...
    ];




xs = xs(1:end-1, :);
ysRaw = GestureNumber(1:end-1) + 1;

%ys = ysRaw == 3;
ys = dummyvar(ysRaw);