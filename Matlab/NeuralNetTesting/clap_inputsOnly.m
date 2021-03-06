function [Y,Xf,Af] = clapFunction_inputsOnly(X,Xi,~)
%MYNEURALNETWORKFUNCTION neural network simulation function.
%
% Generated by Neural Network Toolbox function genFunction, 08-Sep-2016 15:06:40.
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
%   Each Ai{2,ts} = 1xQ matrix, initial states for layer #2.
%
% and returns:
%   Y = 1xTS cell of 1 outputs over TS timesteps.
%   Each Y{1,ts} = 1xQ matrix, output #1 at timestep ts.
%
%   Xf = 1x2 cell 1, final 2 input delay states.
%   Each Xf{1,ts} = 5xQ matrix, final states for input #1.
%
%   Af = 2x0 cell 2, final 0 layer delay states.
%   Each Af{1ts} = 10xQ matrix, final states for layer #1.
%   Each Af{2ts} = 1xQ matrix, final states for layer #2.
%
% where Q is number of samples (or series) and TS is the number of timesteps.

%#ok<*RPMT0>

% ===== NEURAL NETWORK CONSTANTS =====

% Input 1
x1_step1_xoffset = [-165.0625;-59.9375;-39.08;-32.28;-24.1];
x1_step1_gain = [0.00863697705802969;0.0141280353200883;0.0374882849109653;0.0326530612244898;0.0282525780477469];
x1_step1_ymin = -1;

% Layer 1
b1 = [-11.168412460724072;1.563615847399022;1.3582726404709111;-0.56237535524927118;-2.7432294007176719;1.6120024279930238;-11.94465154529529;-3.7686072558478165;1.3518136404249894;-3.0786947704519241];
IW1_1 = [-0.2978946345162114 11.700177235411603 9.8393854598867527 -3.4240834305658612 5.4122011563225421 1.7844468303434466 -2.1242079409405394 9.4994200768699688 -0.85018516030455515 3.570830477783713;-4.2329828244501764 -1.415420919723952 1.0160904636357451 -0.73343363076628265 2.2220005639255689 3.4466898630607892 2.8873522685952744 0.4117148969858464 -0.91000244744609704 0.92205675040100743;-2.2261289824760064 2.1362838866849945 6.1431707347735465 -3.783813302395675 0.51948429131976659 1.0076394071486507 0.37443386081204649 -3.6029979180943608 -4.6358481310213335 12.712795681362689;3.8200537890730173 0.11233065106903323 -2.2486103592250646 0.27222276899515618 12.591305845614336 -11.819930295203834 -0.64584789385650554 4.3116095104094923 -4.3751464200966819 5.0024195722044418;-5.9195760948240324 3.988812283272003 2.5369533209397641 4.555263312277253 -1.362168117305435 1.6228697537402181 -4.8458490091988855 1.9653569001925011 -2.8766617671243377 -2.6897356066676008;-2.0568864090180941 -1.1507417971035274 0.81835658789096333 -0.58425347320190302 2.0297443071602324 1.9440085424518587 2.797578521272988 0.83242117754141054 0.038910261605484976 1.6206328367262779;0.35311833315253621 5.2961142800016381 0.73000095065466153 0.089136871344420748 -8.9899952908203407 -1.7528735948800298 -4.4731486314748894 10.563474809796791 -0.085047065073247732 -5.9094458577165785;10.198828635034918 -21.389707301745581 -8.0751032475731925 -7.223092674550049 -1.6121740297973841 -10.442983332685806 23.146616920141302 -3.0771788661388562 -8.8964711351321171 -24.075020051964593;-0.15750893817441419 -0.56695155344782999 -0.0010652261133754781 0.78285948354892831 0.11098730866687122 -0.51681535246094401 1.4123144898218971 0.88694240786301937 0.31220723869482248 0.70193864577001008;2.3814159767419931 -0.42717177535540662 -1.2722602632538667 0.13624300039470949 -2.1554628907598392 -3.7228535356342976 -1.3647188669250925 1.1007218585018406 -1.3012248650188611 -2.7052662791447029];

% Layer 2
b2 = -3.659427088370987;
LW2_1 = [-0.54503429701052519 2.3347580386523172 0.50199550872237153 0.41873945323319861 -0.61935480623678285 -4.4504660790421964 0.77738716045298883 0.40133326506139599 5.2217604470414685 -1.746452279626737];

% Output 1
y1_step1_ymin = -1;
y1_step1_gain = 2;
y1_step1_xoffset = 0;

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
