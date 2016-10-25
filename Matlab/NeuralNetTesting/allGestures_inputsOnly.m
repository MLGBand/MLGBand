function [Y,Xf,Af] = allGestures_inputsOnly(X,Xi,~)
%MYNEURALNETWORKFUNCTION neural network simulation function.
%
% Generated by Neural Network Toolbox function genFunction, 08-Sep-2016 14:34:52.
%
% [Y,Xf,Af] = myNeuralNetworkFunction(X,Xi,~) takes these arguments:
%
%   X = 1xTS cell, 1 inputs over TS timesteps
%   Each X{1,ts} = 5xQ matrix, input #1 at timestep ts.
%
%   Xi = 1x2 cell 1, initial 2 input delay states.
%   Each Xi{1,ts} = 5xQ matrix, initial states for input #1.
%
%   Ai = 2x0 cell 2, initial 2 layer delay states.
%   Each Ai{1,ts} = 10xQ matrix, initial states for layer #1.
%   Each Ai{2,ts} = 8xQ matrix, initial states for layer #2.
%
% and returns:
%   Y = 1xTS cell of 1 outputs over TS timesteps.
%   Each Y{1,ts} = 8xQ matrix, output #1 at timestep ts.
%
%   Xf = 1x2 cell 1, final 2 input delay states.
%   Each Xf{1,ts} = 5xQ matrix, final states for input #1.
%
%   Af = 2x0 cell 2, final 0 layer delay states.
%   Each Af{1ts} = 10xQ matrix, final states for layer #1.
%   Each Af{2ts} = 8xQ matrix, final states for layer #2.
%
% where Q is number of samples (or series) and TS is the number of timesteps.

%#ok<*RPMT0>

% ===== NEURAL NETWORK CONSTANTS =====

% Input 1
x1_step1_xoffset = [-161.0625;-78.9375;-12.59;-12.52;-9.69];
x1_step1_gain = [0.00784506006374111;0.0129136400322841;0.101419878296146;0.091533180778032;0.0611060189428659];
x1_step1_ymin = -1;

% Layer 1
b1 = [-0.38233610690175668;0.1772531168743052;1.6616623938267336;1.2167595785237679;0.46528693362346352;-1.1079007905005129;-4.4694165678310247;-1.8821659067449534;3.3624788658571618;1.1604033933817126];
IW1_1 = [-0.24193837212016739 -1.2227604841153328 2.3483252127101339 -0.49913612326909162 0.63008558265350978 -0.081788977257819376 -1.0873900538799977 2.2553954165520298 -0.66482173653872245 -0.36416853393977655;0.43167568220250968 2.1035750073043844 -1.765065853077433 1.2875426007169637 2.3212756148189468 -0.40118000753582705 3.5655874642201497 -1.7694304733695303 0.8240651299197792 2.9098408840147796;-1.5629109681899875 -2.616688050812455 -1.0039357048555448 -0.45127874179776106 1.9338607204163334 -1.7826880419960374 -5.3967470247050118 -0.92695014400504339 -0.63396567849490659 0.49830166153658306;0.058733059871461685 1.1830754081102786 -0.23863135269610725 0.83564636612954313 0.44327266333743515 0.37585445260110306 2.9240206096222932 -0.52537158140984475 1.1596751994393191 0.39908036526263302;-0.64926715198064211 -0.78364044631088481 3.2559619542722475 -0.46135834535289172 0.77446614216648035 -5.9585175485846253 -1.4377234012919999 2.3880622655350909 -1.4573628541975534 -0.75537847898121102;0.11006096831575998 -1.1079235896641368 -1.8737362193891323 -0.4517979744501297 -0.94252353224305496 0.10177627568021998 -3.9041645740386239 -0.7954781467100368 -0.47487456059872118 -0.86507654083672014;-0.048915393623609388 3.2569743909338729 5.2998250732136993 -1.1174119144736843 1.220010667339811 0.47179313202275791 -2.3087705458911199 7.7097782169981519 -2.0269074076806808 0.09658287819938749;0.21166355208670684 0.18327194113913778 2.3018311913877412 -0.4369759098801238 -0.79385101752824105 -3.7695843579632098 0.074386376272631921 1.4507606675862426 -0.94019709588454392 -1.0778363616125262;0.8713348459500776 0.2372034725703665 -1.8247785002871 1.1886857107338031 0.95357446607042895 2.1467577890776819 0.95284387085248645 -0.78801284790749326 1.7409888817962387 2.2927523533592105;0.20441707928012753 0.10495568287101451 -1.5739484211387051 0.28844174495794617 0.28143149730739653 2.8235083351850765 0.086853329347702693 -0.99235411133553053 0.74517156900689552 0.6619032821866272];

% Layer 2
b2 = [-1.4281000368726502;-0.17149164434897252;-0.80820372256201456;-1.4709775310898849;-0.09056584360970979;-0.51259846304265533;-0.32381871020889724;-1.1942440482705516];
LW2_1 = [0.19130250951126332 0.2654918046889273 -0.27572285339985725 0.14739229831079539 -0.16783717164331352 0.46785290516614381 -0.57904419844298904 -2.7631984679474382 1.5494058358534926 -4.7492191358114768;0.33257881203641865 0.16114813136554257 -0.13705071827815302 1.6492860995469161 0.26389281923253582 1.424812433559457 0.75280528215496534 -0.040643897721582285 -0.35462025953399395 0.25985038502772723;-0.42531565053757631 -0.38726581335353427 0.27526773050219577 -0.195815779925125 -0.070186335196545588 -0.42603687296098236 0.57504480493061727 -0.40463457620363685 0.25316346076464596 -0.42626159848990663;1.0331408348733433 -0.070126239840189369 0.092421080108883114 0.16978670537326387 -0.76156419859557456 -0.0082991779801988852 -0.49078034112394703 1.3809762240132621 0.40285421349656836 1.1952434234486906;0.32637865421620005 0.024978408216116032 0.043555604574954929 0.19376323642508592 0.13430778579566785 -0.062306679313400164 -0.13902165731130517 0.88105931803834447 -1.6032514996273228 1.7053794171138201;-0.96868530150268417 -0.094628992105251802 0.0077489564154464303 -0.84785918284954054 0.98309606294085183 0.065022379221531576 -0.1019106557538865 3.1178789732868868 -0.40062282559180501 4.0115504249327358;0.12611372216596364 1.228325761145812 0.40494266832946607 0.098720593993181976 0.083224916430624407 -0.33214248644187738 -0.021405968625197176 -0.17774206855509578 0.27857723488792602 -0.058043461715045999;-0.61551358075815477 -1.127923060119973 -0.41116246825459862 -1.2152739708551288 -0.46493387897459537 -1.1289025012335006 0.004312734174513274 -1.9936955053674876 -0.12550616025228378 -1.9384994549674417];

% Output 1
y1_step1_ymin = -1;
y1_step1_gain = [2;2;2;2;2;2;2;2];
y1_step1_xoffset = [0;0;0;0;0;0;0;0];

% ===== SIMULATION ========

% Format Input Arguments
isCellX = iscell(X);
if ~isCellX, X = {X}; end;
if (nargin < 2), error('Initial input states Xi argument needed.'); end

% Dimensions
TS = size(X,2); % timesteps
if ~isempty(X)
    Q = size(X{1},2); % samples/series
elseif ~isempty(Xi)
    Q = size(Xi{1},2);
else
    Q = 0;
end

% Input 1 Delay States
Xd1 = cell(1,3);
for ts=1:2
    Xd1{ts} = mapminmax_apply(Xi{1,ts},x1_step1_gain,x1_step1_xoffset,x1_step1_ymin);
end

% Allocate Outputs
Y = cell(1,TS);

% Time loop
for ts=1:TS
    
    % Rotating delay state position
    xdts = mod(ts+1,3)+1;
    
    % Input 1
    Xd1{xdts} = mapminmax_apply(X{1,ts},x1_step1_gain,x1_step1_xoffset,x1_step1_ymin);
    
    % Layer 1
    tapdelay1 = cat(1,Xd1{mod(xdts-[1 2]-1,3)+1});
    a1 = tansig_apply(repmat(b1,1,Q) + IW1_1*tapdelay1);
    
    % Layer 2
    a2 = repmat(b2,1,Q) + LW2_1*a1;
    
    % Output 1
    Y{1,ts} = mapminmax_reverse(a2,y1_step1_gain,y1_step1_xoffset,y1_step1_ymin);
end

% Final Delay States
finalxts = TS+(1: 2);
xits = finalxts(finalxts<=2);
xts = finalxts(finalxts>2)-2;
Xf = [Xi(:,xits) X(:,xts)];
Af = cell(2,0);

% Format Output Arguments
if ~isCellX, Y = cell2mat(Y); end
end

% ===== MODULE FUNCTIONS ========

% Map Minimum and Maximum Input Processing Function
function y = mapminmax_apply(x,settings_gain,settings_xoffset,settings_ymin)
y = bsxfun(@minus,x,settings_xoffset);
y = bsxfun(@times,y,settings_gain);
y = bsxfun(@plus,y,settings_ymin);
end

% Sigmoid Symmetric Transfer Function
function a = tansig_apply(n)
a = 2 ./ (1 + exp(-2*n)) - 1;
end

% Map Minimum and Maximum Output Reverse-Processing Function
function x = mapminmax_reverse(y,settings_gain,settings_xoffset,settings_ymin)
x = bsxfun(@minus,y,settings_ymin);
x = bsxfun(@rdivide,x,settings_gain);
x = bsxfun(@plus,x,settings_xoffset);
end
