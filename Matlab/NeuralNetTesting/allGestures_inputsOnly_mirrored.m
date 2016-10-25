function [Y,Xf,Af] = allGestures_inputsOnly_mirrored(X,Xi,~)
%allGestures_inputsOnly_mirrored neural network simulation function.
%
% Generated by Neural Network Toolbox function genFunction, 12-Sep-2016 15:29:42.
%
% [Y,Xf,Af] = myNeuralNetworkFunction(X,Xi,~) takes these arguments:
%
%   X = 1xTS cell, 1 inputs over TS timesteps
%   Each X{1,ts} = 8xQ matrix, input #1 at timestep ts.
%
%   Xi = 1x2 cell 1, initial 2 input delay states.
%   Each Xi{1,ts} = 8xQ matrix, initial states for input #1.
%
%   Ai = 2x0 cell 2, initial 2 layer delay states.
%   Each Ai{1,ts} = 24xQ matrix, initial states for layer #1.
%   Each Ai{2,ts} = 9xQ matrix, initial states for layer #2.
%
% and returns:
%   Y = 1xTS cell of 1 outputs over TS timesteps.
%   Each Y{1,ts} = 9xQ matrix, output #1 at timestep ts.
%
%   Xf = 1x2 cell 1, final 2 input delay states.
%   Each Xf{1,ts} = 8xQ matrix, final states for input #1.
%
%   Af = 2x0 cell 2, final 0 layer delay states.
%   Each Af{1ts} = 24xQ matrix, final states for layer #1.
%   Each Af{2ts} = 9xQ matrix, final states for layer #2.
%
% where Q is number of samples (or series) and TS is the number of timesteps.

%#ok<*RPMT0>

% ===== NEURAL NETWORK CONSTANTS =====

% Input 1
x1_step1_xoffset = [-12.59;-12.52;-9.69;-161.0625;-78.9375;-9.63;-6.85;-9.26];
x1_step1_gain = [0.101419878296146;0.091533180778032;0.0611060189428659;0.00784506006374111;0.0129136400322841;0.104547830632514;0.12012012012012;0.105097214923805];
x1_step1_ymin = -1;

% Layer 1
b1 = [-5.4206856143338324;1.9702314952958775;-20.80714287654942;-9.8863164087731672;0.98093957269134013;1.2603242223616198;-2.6999469235603151;-2.5057907434954889;1.5872783596263234;4.0593360184997644;1.4526008157703949;0.5274316581647378;1.4555735244507528;-0.2454619115000945;-2.6991922858363582;-14.694830044481021;0.76037896096377278;-2.6998093182098675;-1.5510883375761724;1.6210384982926582;0.96397071031475112;14.011108063752546;3.1365074356889222;-69.166505591115751];
IW1_1 = [1.3154908551463147 3.2037627877585653 -9.954191203091554 0.9994416047763176 2.8799360208559506 -14.085819180347412 1.8370355176557958 1.694716573073777 4.501250858466495 -3.1435820647738408 7.0742469029406436 -4.7516987001869362 6.181660434715627 9.5068956421397459 -9.1494006425742604 -6.2669103548362086;-1.0967687136550595 -5.7255126972577086 4.6215310822689037 -3.2160739518608912 7.8831055321160051 -4.3536361044971716 -0.92583471013923102 3.3901257039958828 0.57427668770602336 0.16435779317301463 0.50256706569394016 5.8915722456341655 -4.091099927732782 1.4004335767939455 1.5107475584464043 -3.4475366901221944;-12.045470317912875 1.4021159421073062 -7.5971931842096474 16.673926634044207 3.5130700440898006 0.73000440700518354 3.7402521617927196 8.6182797119074124 6.2511640027289017 -2.9169330398184932 -7.8502836818336252 -9.8484795067918451 20.120675081245004 -20.831795173632116 2.0380425099356092 1.3589635700533635;12.2273651003943 1.2377081505516214 4.3264722341119359 6.9250965840899612 -10.492996241113449 13.222823653840429 -1.4225170455947527 4.7366614069873165 -3.1159596308262332 0.49565650430262542 -1.7130956261254735 -12.382716975707982 -2.8076823879046757 -3.8304033382114331 2.23138446760814 3.0728352315871836;-2.6443552524608007 -4.5245270203834682 2.6564378097325698 1.1386647429965258 5.6747545394864476 -3.7132138906483196 1.2452834721946988 2.8180759065526702 0.44728282590444607 0.59848968039313899 1.2415158012761014 5.2690953838173344 -11.180051337216907 9.8530931856962667 0.76639288691353313 -1.600515303733806;0.70956366794911796 -1.2777180244505004 -3.6440937454658782 -3.1785896304148635 4.5404626208877064 -4.031420545280028 -5.4331047605115348 -0.772338455855345 0.93109209701339135 -1.0308083547790239 1.1737950510110897 0.043032008302005098 -5.4970508593159746 4.7374973495224157 4.5902727091518951 0.28334955007066365;29.90521179715951 0.59803173180696279 4.8747532906916415 2.3487572794848184 -1.5365781783282073 -10.360755407731755 4.1854126875140807 -5.2086969727461678 -0.82018058457808607 -3.0388486081879922 -0.69696949011452791 -9.8813208815053368 15.12562611719369 2.1604399967455912 -9.2239982364452349 2.0844491032967927;-0.33840172496031906 -0.16893314761816597 1.2493992695266072 0.11821629051117921 -0.066215348674890206 -0.52834436613978653 0.80965042272398635 1.954370981708963 -0.96049802426024289 0.0014167747354432657 -0.96113716087882928 0.47372757291631568 -1.5405923614395576 -1.6257118697827693 1.0672824016068339 -0.11714599549143906;1.5808468462001692 -3.5806590921259889 1.2694034211722429 -2.7417620301185575 12.824661815947746 -12.339621015546214 0.37589187477441433 -0.58821442157292325 -1.2873994117896914 -1.3621078192022955 -1.5976495056249718 -2.1236300610838583 0.37235752164485181 1.2740901382587815 -2.6138094600013275 1.6082688032542409;-32.199901631881964 7.530776057287305 -2.1024247892851209 -3.5550399598235183 -5.1249786908897033 11.072597209542204 -2.1090623368972259 1.6969545048334249 0.0603627285660399 0.81394736639665699 -0.5136169083773523 9.0667207178266072 -21.258235478722387 9.1870365033351931 2.3841378185530653 -1.3016337147334873;-2.0189189037762718 -0.89376589338053236 -2.8688226440685543 1.2638732637648586 -1.01866139775568 -7.3081513478234772 2.5205587528352034 -1.6848100741424958 -0.9420390301977134 0.99629000407546486 -0.39611859937905813 -0.59038116798425833 8.9114741918198472 3.2752810903387153 -3.3383551036933024 1.1840492580782849;1.6014536966122539 -0.65728197817881839 -0.30619760722312034 -5.3879091343114762 -5.0594598952449248 6.6176771856281329 0.4249510393214922 0.59953064487250862 1.3240643398326715 -0.19240655510748261 -0.1750061671839141 6.6979140185757995 8.1568897042624258 -9.7873529813145375 0.20274322749147994 -3.5361999718257238;-0.89486615594111063 -0.77925900605287846 -0.25466519612945038 -1.444760381725293 -1.1402148355568611 1.8315207034041845 2.0081072876403718 -0.82037498212799731 1.1736200085832744 0.070385771064964187 1.093849239684008 -0.46444261573627921 -2.3331712340887201 3.9169200110127544 -3.2182852851774522 0.93509750481723375;-4.0498000019585376 -3.6677283061822674 -0.63203622001275839 -1.8300594744941239 -4.3843620301622188 -1.0341615426323298 0.94198760901072132 3.639637247295104 -0.38817307898048142 0.89290330574465604 1.558224166851891 7.7973607389269572 3.4427792118758789 2.4557628382986909 1.265487184748072 -2.7350676666275393;1.286281686567313 0.063927206178239077 0.11734168492689556 -4.1678351439916081 -5.3791167405909048 -3.9088335763450086 0.59629417041597454 2.0409173268092191 0.22660324330647721 0.096375792019430362 -1.2166143630745434 6.6125426696122886 10.982220106026718 -0.17031031558613605 0.5194057020697187 -2.07065255237847;0.93064428589532644 8.3137372376496241 0.43864874394438341 4.6680943188860606 29.728677240121247 -11.742305388687617 -9.9131018333910568 11.03282686960303 3.7071197744115856 0.069542310142123032 -0.057409463209093806 0.7428480693484879 -2.5993106304194913 -5.1213409241764829 15.352822594570757 -1.4846286357675482;-2.6909074157876249 0.39043187027580195 1.4168629614331683 -7.7839277845709782 -0.21549386325054765 5.6868551564834888 2.8906862103558839 1.9905522140265797 0.80742053209292686 -1.1328621915277626 0.77102013368967193 2.7095317085490191 -9.5418811557794854 0.4670719898107395 -3.4221376979106264 -1.7189588800186346;1.6247178629842869 1.7890403137106814 2.4805150827080595 -2.8187663317976357 -12.691803720901865 -0.27961784863332589 2.03670204818058 5.6747309689921739 -1.0614013181812343 0.78762903750918423 -1.3394274523329284 7.7549614475849831 18.26909298033577 -4.7716169955782606 0.08628989072116855 -4.3044038507535776;0.58138792595626987 1.3850687800269952 4.4740577899788416 -0.093128459380826589 -0.63818642713336249 1.2514176538906714 1.3149064445277181 5.0381298945684705 -0.25110404808286668 -0.030231281431877945 -1.2356635474477207 3.1922352751916359 1.6540893636247833 -1.5380267913404964 0.074608511813026246 -2.0134723819372873;-1.1310870285809191 -0.68289665199052196 2.6101822485043389 0.25525990654537445 -3.9764714891331292 8.1771080323049752 -1.5605144665077952 -0.75019935424804851 2.9454928849497368 -1.0872045298801825 4.3563826135529631 2.2425865403813572 3.7432277605757469 -5.8286218946054156 2.892859480388577 -0.15196786705285464;-0.50653220621659678 -1.0574971265015574 -2.4679860955438322 0.29401330717973417 2.8691415432221596 -2.8694367641607186 -0.82747748898679685 -2.907095479697416 0.81426310931750656 -0.10746185616814048 1.1212691594809601 -1.8156796925964027 -5.972930284901584 5.5735194908481613 0.48743302917806697 1.9548880168562184;6.9017793988334466 -0.057955306701003517 0.9314430851051323 -1.1262477320777997 -17.494532188549559 9.3225173906585947 -2.4576599354013626 -4.9846820952574351 -6.7765842044899358 0.9534942861246235 1.8591936879755435 9.1835016301475569 -10.385702882346235 13.35294847624354 2.4231239311955108 -7.4498991273279751;9.3452950367591967 25.816874318013632 14.267949845244129 -48.230733479752608 5.3313291096772097 21.639519716753778 -6.2306443518658927 38.652401226058963 52.991250756375791 -20.519623442631797 -23.703305797095446 97.087337094012526 -13.086694375104081 -16.852333167966172 44.257871854554054 -60.023258498311208;1.8469806846651913 4.941134302242526 -5.6133058621529495 12.927336512352666 91.469559428195453 -64.957928993357271 8.5819948986264407 8.8745556801377887 0.36270651403955434 -0.11046087406391034 1.5294218617776896 0.60977306887197402 25.257565605509551 -6.059211259870481 -2.4678279376775305 54.756719244894953];

% Layer 2
b2 = [-2.0812711922057767;-0.3388830767174863;-0.42731241466801156;-0.32759991449783205;-1.516790970318191;-0.0037551657828298682;-0.21322580676746269;-1.384954316463564;-0.7062071425788139];
LW2_1 = [0.056304688450192993 -0.61461948927742183 0.76545006292002837 -0.76037013214780258 0.80249271649248421 -0.016670863111634938 -0.42529056737586834 0.36286874047294582 0.11426477855199543 -0.53640377386428639 0.13712638871131463 0.66086881481114612 -0.26836328624284445 -0.70235472370803831 -0.4550391195801336 -0.35982037904977909 -0.3158815894943236 0.45061336725833501 1.05549909605715 0.43408272758506405 0.86298495715427403 0.70847665837613683 -0.24305824152087321 -0.20874405211940572;0.035508200166818507 -0.33007477331687518 -0.24291893358610511 0.087947682564883778 -0.14559041684891671 -0.35613345927240492 0.10267113335575655 0.12036709317727946 0.38586549879259024 0.11282477716164574 -0.79807180556943491 0.4217324108948477 0.2616380324620054 0.43870318792284618 -0.64517017414451228 0.14294339322796148 -0.5883236912089661 0.25822104924378259 0.20247995345636749 0.0079382957595968348 0.662146537209785 -0.89761997671761007 -0.058062343575063104 -0.10027292283464176;-0.04995590673802356 -0.21765429108576151 -0.30294229761078778 1.0021669439204532 0.29137370530429813 0.72222390644333201 0.028232562661635447 0.33482000526352246 -0.29420427199669302 0.18568574900833545 1.0716258350577168 -0.19459920165392891 -0.1774525454668337 -0.15864933273895004 0.53876791442429572 -0.30313328837765785 0.44991190887983856 -0.567513976809362 -0.18486716476383122 0.098523761261908205 -0.95453118514338042 -0.44403753349984465 0.01666665131846911 -0.030477330900204637;0.82750214460257199 0.10384851936052136 -0.056009091403285804 0.09426245016419639 0.036097121760933937 -0.82837575311858858 0.10501077002184814 -0.89966858995769783 0.2278907532995092 0.01242813907055945 0.09500678173930853 -0.43557152575800917 -1.0765542206540968 -0.083449835542609527 -0.11822783920636352 0.1500496615872994 0.5290402262839583 0.047313611955773292 0.45028070671870829 -0.21221535106330397 1.3567540238698792 0.06329358086392628 -0.0018917411595347671 -0.02518480278216869;-0.80881666851655298 0.01923688609646396 0.029102891274340167 -0.02470961698035895 0.03646218249254618 0.27034101382751763 -0.019707666458052817 -0.46273673048431124 -0.035048335315801087 -0.058611149008332489 0.44772941748940998 0.56891670510459813 -0.20866861904050782 0.20520868540449846 -0.075719539117776694 0.10788341056246005 0.34311787629761847 0.081383798096203191 -0.36042342771776337 0.030256970668713769 -0.7157647063349476 0.1362222904252619 0.013991741541434783 -0.011039650815978557;0.0049971729300744708 -0.071708343031771926 -0.099484812379136797 -0.026592490070280056 -0.26169958557481543 -0.029462669084464613 -0.01009207273317143 0.26948842178947108 0.042870659353237112 -0.060049636468416809 -0.22835996775431264 -0.1846304905273394 0.42368110729148312 0.34628648241712134 1.0026984575274918 0.046065586418440579 -0.28427366682087368 -0.35350110161517645 -0.65018187040262787 0.0055932089040738129 -0.81952563910250431 -0.013171542116025262 0.15737603597892069 -0.059330287611068733;0.00019812090398928228 0.12290882886965074 -0.051050448219548968 -0.052649679600787362 0.52741290663081652 0.14724617292615999 -0.028225002690051175 0.31032500818687081 -0.0048769833311672443 0.098801375918822334 -0.65437051432499249 -0.44336212947571757 0.36600679171446998 -0.59659611043873384 -0.03812162329482767 0.080248523292699464 0.20781771314150291 0.31793476288056582 -0.81556518720547755 0.015737999843234259 -0.475384878676889 0.10530091792918389 -0.0041678025680583998 0.040360565200889251;-0.10736524516241196 0.58721414537698879 0.040228566379804473 -0.27979024333095343 -0.59988693187071473 0.30021982378781303 0.17887514438758892 0.21085346189043064 -0.42057458237014433 0.15029765431717698 0.16186572333421487 -0.050433736010652944 0.53468419642177589 0.10073153964313286 0.10806893121245516 0.30807654189089068 -0.27578973296982079 -0.26873395940183992 0.171798910086012 -0.4707691570162108 -0.19410112270484831 0.2270109870350614 -0.092320376320927142 -0.17785877558254876;0.041627493363365776 0.40084851700820723 -0.082375937375171543 -0.040264914519412623 -0.68666169838668001 -0.20938817239785931 0.068525698830324583 -0.24631741033851101 -0.016187516983430864 0.09502686386450078 -0.23255185868343975 -0.34292084738495726 0.14502854351467548 0.45012010704075056 -0.31725700782071414 -0.17231344955229411 -0.065619044109091362 0.034282448391725923 0.13097898377140116 0.090851544056869959 0.27742201372859715 0.11452461770397274 0.21146607630565509 0.57254725744508728];

% Output 1
y1_step1_ymin = -1;
y1_step1_gain = [2;2;2;2;2;2;2;2;2];
y1_step1_xoffset = [0;0;0;0;0;0;0;0;0];

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