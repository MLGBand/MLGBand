# import wx
# from wx import glcanvas
# from OpenGL.GL import *
# from OpenGL.GLUT import *
# from OpenGL.GLU import *
from PIL.Image import *
from time import *
from DeviceConnect import DeviceConnection
from random import randint

# This working example of the use of OpenGL in the wxPython context
# was assembled in August 2012 from the GLCanvas.py file found in
# the wxPython docs-demo package, plus components of that package's
# run-time environment.

# This code contains concepts built upon from a freely available code
# example found here: https://wiki.wxpython.org/GLCanvas%20update
# This has been modified to suit the demo app for Team MLG's GRAPHI project

try:
    import wx
    from wx import glcanvas
    haveGLCanvas = True
except ImportError:
    haveGLCanvas = False

try:
    # The Python OpenGL package can be found at
    # http://PyOpenGL.sourceforge.net/
    from OpenGL.GL import *
    from OpenGL.GLUT import *
    from OpenGL.GLU import *
    haveOpenGL = True
except ImportError:
    haveOpenGL = False

#----------------------------------------------------------------------

buttonDefs = {
    wx.NewId() : ('connectButton',      'Connect'),
    wx.NewId() : ('disconnectButton',      'Disconnect'),
    wx.NewId() : ('gameButton',      'Play Game'),
    wx.NewId() : ('calibrateButton',      'Calibrate'),
    }

class ButtonPanel(wx.Panel):
    def __init__(self, parent):
        wx.Panel.__init__(self, parent, -1)
        
        # Primary panel
        box = wx.BoxSizer(wx.VERTICAL)
        horizontals = wx.BoxSizer(wx.HORIZONTAL)
        
        # Create main panels
        tcpPanel = wx.BoxSizer(wx.VERTICAL)
        probabilityPanel = wx.BoxSizer(wx.VERTICAL)
        filePanel = wx.BoxSizer(wx.VERTICAL)
        currentGesturePanel = wx.BoxSizer(wx.HORIZONTAL)
        
        
        # Create sub panels
        tcpConnectPanel = wx.BoxSizer(wx.HORIZONTAL)
        
        #Add IP text label
        ipLabel = wx.StaticText(self, -1, "IP Address: ")
        tcpConnectPanel.Add(ipLabel, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        # Add text panel
        self.txt = wx.TextCtrl(self, -1, size=(85,-1))
        self.txt.SetValue('192.168.4.1')
        tcpConnectPanel.Add(self.txt, 0, wx.ALIGN_CENTER|wx.VERTICAL, 30)

        #box = wx.BoxSizer(wx.VERTICAL)
        # tcpConnectPanel.Add((20, 30))
        keys = buttonDefs.keys()
        sorted(keys)
        # Add buttons
        for k in keys:
            text = buttonDefs[k][1]
            btn = wx.Button(self, k, text)
            if text == "Play Game" or text == "Calibrate":
                filePanel.Add(btn, 0, wx.ALIGN_CENTER|wx.ALL, 15)
            else:
                tcpConnectPanel.Add(btn, 0, wx.ALIGN_CENTER|wx.ALL, 15)
            self.Bind(wx.EVT_BUTTON, self.OnButton, btn)
        
        tcpPanel.Add(tcpConnectPanel, wx.EXPAND)
        
        #Add connection status text label
        self.connectionLabel = wx.StaticText(self, -1, label = "Connection Status: Disconnected")
        tcpPanel.Add(self.connectionLabel, 0, wx.ALIGN_CENTRE|wx.VERTICAL)
        
        # Add probabilities
        self.p1l = wx.StaticText(self, -1, label = "P(Rest) = 0")
        probabilityPanel.Add(self.p1l, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        self.p2l = wx.StaticText(self, -1, label = "P(Push) = 0")
        probabilityPanel.Add(self.p2l, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        self.p3l = wx.StaticText(self, -1, label = "P(Pull) = 0")
        probabilityPanel.Add(self.p3l, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        self.p4l = wx.StaticText(self, -1, label = "P(Twist in) = 0")
        probabilityPanel.Add(self.p4l, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        self.p5l = wx.StaticText(self, -1, label = "P(Twist Out) = 0")
        probabilityPanel.Add(self.p5l, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        self.p6l = wx.StaticText(self, -1, label = "P(Lift) = 0")
        probabilityPanel.Add(self.p6l, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        self.p7l = wx.StaticText(self, -1, label = "P(Hit) = 0")
        probabilityPanel.Add(self.p7l, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        self.p8l = wx.StaticText(self, -1, label = "P(Drag Left) = 0")
        probabilityPanel.Add(self.p8l, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        self.p9l = wx.StaticText(self, -1, label = "P(Drag Right) = 0")
        probabilityPanel.Add(self.p9l, 0, wx.ALIGN_CENTRE|wx.ALL)
        
        horizontals.Add(tcpPanel)
        horizontals.Add(probabilityPanel)
        horizontals.Add(filePanel)
        box.Add(horizontals)
        
        #Add gesture status text label
        self.gestureLabel = wx.StaticText(self, -1, label = "Current Gesture: None")
        self.gestureLabel.SetForegroundColour(wx.RED)
        font = wx.Font(18, wx.DECORATIVE, wx.BOLD, wx.NORMAL)
        self.gestureLabel.SetFont(font)
        
        box.Add(self.gestureLabel, 0, wx.ALIGN_CENTRE|wx.VERTICAL)
        
        

        # With this enabled, you see how you can put a GLCanvas on the wx.Panel
        if 1:
            self.c = CubeCanvas(self)
            self.c.SetMinSize((300, 300))
            # box.Add(c, 0, wx.ALIGN_CENTER|wx.ALL, 15)
            box.Add(self.c, 1, wx.EXPAND)

        self.SetAutoLayout(True)
        self.SetSizer(box)

    def OnButton(self, evt):
        if not haveGLCanvas:
            dlg = wx.MessageDialog(self,
                                   'The GLCanvas class has not been included with this build of wxPython!',
                                   'Sorry', wx.OK | wx.ICON_WARNING)
            dlg.ShowModal()
            dlg.Destroy()

        elif not haveOpenGL:
            dlg = wx.MessageDialog(self,
                                   'The OpenGL package was not found.  You can get it at\n'
                                   'http://PyOpenGL.sourceforge.net/',
                                   'Sorry', wx.OK | wx.ICON_WARNING)
            dlg.ShowModal()
            dlg.Destroy()

        else:
            buttonID = buttonDefs[evt.GetId()][0]
            if buttonID == "connectButton":
                # print (self.c.getTCPStatus)
                if not self.c.getTCPStatus():
                    ip = self.txt.GetValue()
                    print ("Connecting via TCP to IP address: " + ip)
                    self.c.InitTCP(ip, self.connectionLabel, self.gestureLabel, self.p1l, self.p2l, self.p3l, self.p4l, self.p5l, self.p6l, self.p7l, self.p8l, self.p9l)
                    #"127.0.0.1"
            elif buttonID == "disconnectButton":
                if self.c.getTCPStatus():
                    print("TCP Disconnected")
                    self.c.DisconnectTCP(self.connectionLabel)
                    # self.c.setTCPStatus(False)
            elif buttonID == "gameButton":
                if self.c.getTCPStatus():
                    self.c.startGame()
                else:
                    print("Please connect to device before playing game")
            elif buttonID == "calibrateButton":
                if self.c.getTCPStatus():
                    self.c.Calibrate()
                else:
                    print("Please connect before calibrating")
            else:
                print ("button name not identified")
class MyCanvasBase(glcanvas.GLCanvas):
    def __init__(self, parent):
        glcanvas.GLCanvas.__init__(self, parent, -1)
        self.context = glcanvas.GLContext(self)
        self.init = False
        self.init_resize = False
        self.context = glcanvas.GLContext(self)
        self.timer = wx.Timer(self)
        self.count = 0
        self.LastGesture = 0
        
        # initial mouse position
        self.lastx = self.x = 30
        self.lasty = self.y = 30
        self.size = None
        self.Bind(wx.EVT_ERASE_BACKGROUND, self.OnEraseBackground)
        self.Bind(wx.EVT_SIZE, self.OnSize)
        self.Bind(wx.EVT_PAINT, self.OnPaint)
        self.Bind(wx.EVT_TIMER, self.OnTimer)
        # self.Bind(wx.EVT_LEFT_DOWN, self.onIdle)
        # self.Bind(wx.EVT_LEFT_UP, self.onIdle)
        # self.Bind(wx.EVT_MOTION, self.onIdle)
        # self.Bind(wx.EVT_IDLE, self.onIdle)
        

    def OnTimer(self, event):
        self.count += 1
        lg = self.getData()
        
        if lg != 0:
            self.OnDraw()
        if self.count == 400:
            self.count = 0
            if self.getGameStatus() == True:
                # print("Timer event")
                self.incrementGame()
            
        
        # self.OnDraw()
        

    def OnEraseBackground(self, event):
        pass # Do nothing, to avoid flashing on MSW.

    def OnSize(self, event):
        wx.CallAfter(self.DoSetViewport)
        event.Skip()

    def DoSetViewport(self):
        # print("Resize firing...")
        size = self.size = self.GetClientSize()
        self.SetCurrent(self.context)
        glViewport(0, 0, size.width, size.height)
        self.canvas_width = size.width
        self.canvas_height = size.height/2
        
    def OnPaint(self, event):
        self.timer.Start(1)
        dc = wx.PaintDC(self)
        self.SetCurrent(self.context)
        if not self.init:
            self.InitGL()
            self.init = True
        self.OnDraw()

    def OnMouseDown(self, evt):
        self.CaptureMouse()
        self.x, self.y = self.lastx, self.lasty = evt.GetPosition()

    def OnMouseUp(self, evt):
        self.ReleaseMouse()

    def OnMouseMotion(self, evt):
        if evt.Dragging() and evt.LeftIsDown():
            self.lastx, self.lasty = self.x, self.y
            self.x, self.y = evt.GetPosition()
            self.Refresh(False)

class CubeCanvas(MyCanvasBase):
    
    def InitTCP(self, ip, connectionLabel, gestureLabel, p1l, p2l, p3l, p4l, p5l, p6l, p7l, p8l, p9l):
        self.device = DeviceConnection()
        self.device.Connect(ip)
        self.tcp_status = True
        connectionLabel.SetLabel("Connection Status: Connected to IP: " + ip)
        self.gestureLabel = gestureLabel
        self.p1l = p1l
        self.p2l = p2l
        self.p3l = p3l
        self.p4l = p4l
        self.p5l = p5l
        self.p6l = p6l
        self.p7l = p7l
        self.p8l = p8l
        self.p9l = p9l
        
    def Calibrate(self):
        self.device.Calibrate()
        
    def getTCPStatus(self):
        return self.tcp_status
        
    def setTCPStatus(self, status):
        self.tcp_status = status
        
    def DisconnectTCP(self, connectionLabel):
        self.device.Disconnect()
        self.tcp_status = False
        connectionLabel.SetLabel("Connection Status: Disconnected")
    def getData(self):
        if self.tcp_status: # Assuming we have a TCP connection
            self.data = self.device.Read() #read from open TCP connection
            i = 19
            self.p1l.SetLabel("P(Rest) = " + str("%.2f" % self.data[i]))
            self.p2l.SetLabel("P(Push) = " + str("%.2f" % self.data[i+1]))
            self.p3l.SetLabel("P(Pull) = " + str("%.2f" % self.data[i+2]))
            self.p4l.SetLabel("P(Twist In) = " + str("%.2f" % self.data[i+3]))
            self.p5l.SetLabel("P(Twist Out) = " + str("%.2f" % self.data[i+4]))
            self.p6l.SetLabel("P(Lift) = " + str("%.2f" % self.data[i+5]))
            self.p7l.SetLabel("P(Hit) = " + str("%.2f" % self.data[i+6]))
            self.p8l.SetLabel("P(Drag Left) = " + str("%.2f" % self.data[i+7]))
            self.p9l.SetLabel("P(Drag Right) = " + str("%.2f" % self.data[i+8]))
            if self.data[17] != 0:
                self.LastGesture = self.data[17]
                if self.gameStatus:
                    self.gesturePerformed = min(self.data[self.gameGestures[self.currentGestureIndex] + 19]/0.7, 1)
                    print(self.gesturePerformed)
            return self.LastGesture
    
    def startGame(self):
            self.gameStatus = True
            self.gameScores = []
            self.gameGestures = []
            # populate with 10 random gestures
            for i in range(0, 10):
                self.gameGestures.append(randint(1,7))
            self.currentGestureIndex = 0
            self.gestureLabel.SetLabel("Do Gesture " + str(self.currentGestureIndex + 1) + ": " + self.gestureStrings[self.gameGestures[self.currentGestureIndex]])
    
    def incrementGame(self):
            # print("Incrementing game")
            self.gameScores.append(self.gesturePerformed)
            self.gesturePerformed = 0.0
            self.currentGestureIndex += 1
            if self.currentGestureIndex == 10:
                # print(self.gameScores)
                self.gameStatus = False
                score = sum(self.gameScores)
                self.gestureLabel.SetLabel("Your score = " + str("%.2f" % score) + "/10")
            else:
                self.gestureLabel.SetLabel("Do Gesture " + str(self.currentGestureIndex + 1) + ": " + self.gestureStrings[self.gameGestures[self.currentGestureIndex]])
                
    
                
    def getGameStatus(self):
        return self.gameStatus
    
    def InitValues(self):
            self.Phi = 100
            self.Theta = 0
            self.Phi_sign = 1

            self.xc = 0.0
            self.yc = -1.0
            self.zc = 0.0

            self.x_sign = 1
            self.y_sign = 1
            self.z_sign = 1
            
            self.gestureComplete = 1
        
    def InitGL(self):
            self.tcp_status = False
            
            self.LastGesture = 0
            self.gestureStrings = ["Rest", "Push", "Pull", "Twist In", "Twist Out", "Lift", "Hit", "Drag Left", "Drag Right"]
            self.gesturePerformed = 0.0
            self.gameStatus = False
            
            self.InitValues()
        
            self.nWoodTexID = 0
            self.oWoodImage = Image( )
            print(self.oWoodImage)
            
            # load images
            self.oWoodImage = self.LoadImage( "Images/realhand-transparent.png" )
            print(self.oWoodImage)
            # init texturing
            self.InitTexturing( )
            
            glClearDepth( 1.0 )
            glDepthFunc( GL_LESS )

            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            
            self.ResizeGLScene( 200, 200 )

            # position viewer
            glMatrixMode(GL_MODELVIEW)
            glTranslatef(0.0, 0.0, -3.0)


            glEnable(GL_DEPTH_TEST)
            
            glShadeModel( GL_SMOOTH )
            
            self.quadratic = gluNewQuadric()
            gluQuadricNormals(self.quadratic, GLU_SMOOTH)        # Create Smooth Normals (NEW)
            gluQuadricTexture(self.quadratic, GL_TRUE)

    def OnDraw(self):
            
        
        # clear color and depth buffers
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
        
        
        glEnable(GL_TEXTURE_2D)
        
        glPushMatrix() #Push Stack matrix down by one - orient cylinder independently of all else
        glBindTexture( GL_TEXTURE_2D, self.nWoodTexID )
        
        glTranslatef(self.xc, self.yc, self.zc)

        glRotatef(-135, 1.0, 0.0, 0.0)
        glRotatef(self.Phi, 0.0, 0.0, 1.0)
        
        glColor4f(1,1,1,1)
        gluCylinder(self.quadratic,0.4,0.4,1.8,40,40)
        
        glPopMatrix()
        
        glEnable(GL_BLEND)
        
        glDisable(GL_TEXTURE_2D)

        self.SwapBuffers()
        
        if self.tcp_status == False:
            self.InitValues
            sleep(0.01)
            return
        
        if self.gestureComplete:
            self.gesture = self.getGestures()
            # self.gestureLabel.SetLabel("Current Gesture: " + str(self.gesture))
            if self.gesture == 4:
                self.Phi_sign = 1
                if not self.gameStatus:
                    self.gestureLabel.SetLabel("Current Gesture: Twist Out")
            elif self.gesture == 3:
                self.Phi_sign = -1
                if not self.gameStatus:
                    self.gestureLabel.SetLabel("Current Gesture: Twist In")
            elif self.gesture == 8:
                self.x_sign = 1
                if not self.gameStatus:
                    self.gestureLabel.SetLabel("Current Gesture: Drag Right")
            elif self.gesture == 7:
                self.x_sign = -1
                if not self.gameStatus:
                    self.gestureLabel.SetLabel("Current Gesture: Drag Left")
            elif self.gesture == 6:
                self.z_sign = 1
                if not self.gameStatus:
                    self.gestureLabel.SetLabel("Current Gesture: Lift")
            elif self.gesture == 5:
                self.z_sign = -1
                if not self.gameStatus:
                    self.gestureLabel.SetLabel("Current Gesture: Hit")
            elif self.gesture == 1:
                self.y_sign = 1
                if not self.gameStatus:
                    self.gestureLabel.SetLabel("Current Gesture: Push")
            elif self.gesture == 2:
                self.y_sign = -1
                if not self.gameStatus:
                    self.gestureLabel.SetLabel("Current Gesture: Pull")
            elif self.gesture == 0:
                if not self.gameStatus:
                    self.gestureLabel.SetLabel("Current Gesture: Resting")            
        #--**Update Phi**----------------------------------------------------
        if self.gesture == 0: #relax
            self.gestureComplete = 1
        elif self.gesture == 4: # twist out
            self.gestureComplete = 0
            self.Phi = self.Phi + self.Phi_sign*1
            if self.Phi > 135:
                 self.Phi_sign = -1
                 self.Phi = self.Phi + self.Phi_sign*1.0
            if self.Phi == 100:
                self.Phi_sign = 1
                self.gestureComplete = 1
                self.LastGesture = 0
        elif self.gesture == 3: #twist in
            self.gestureComplete = 0
            self.Phi = self.Phi + self.Phi_sign*1
            if self.Phi < 45:
                 self.Phi_sign = 1
                 self.Phi = self.Phi + self.Phi_sign*1.0
            if self.Phi == 100:
                self.Phi_sign = 1
                self.gestureComplete = 1
                self.LastGesture = 0
        elif self.gesture == 8: #drag right
            self.gestureComplete = 0
            self.xc += self.x_sign*0.03
            if self.xc > 1:
                self.x_sign = -1
                self.xc += self.x_sign*0.03
            if self.xc <= 0:
                self.x_sign = 1
                self.gestureComplete = 1
                self.LastGesture = 0
        elif self.gesture == 7: #drag left
            self.gestureComplete = 0
            self.xc += self.x_sign*0.03
            if self.xc < -1:
                self.x_sign = 1
                self.xc += self.x_sign*0.03
            if self.xc >= 0:
                self.x_sign = 1
                self.gestureComplete = 1
                self.LastGesture = 0
        elif self.gesture == 6: #lift
            self.gestureComplete = 0
            self.zc += self.z_sign*0.04
            if self.zc > 1.5:
                self.z_sign = -1
                self.zc += self.z_sign*0.04
            if self.zc <= 0:
                self.z_sign = 1
                self.gestureComplete = 1
                self.LastGesture = 0
        elif self.gesture == 5: #hit
            self.gestureComplete = 0
            self.zc += self.z_sign*0.04
            if self.zc < -1.5:
                self.z_sign = 1
                self.zc += self.z_sign*0.04
            if self.zc >= 0:
                self.z_sign = 1
                self.gestureComplete = 1
                self.LastGesture = 0
        elif self.gesture == 1: #push
             self.gestureComplete = 0
             self.yc += self.y_sign*0.03
             if self.yc > 0:
                 self.y_sign = -1
                 self.yc += self.y_sign*0.03
             if self.yc <= -1:
                 self.y_sign = 1
                 self.gestureComplete = 1
                 self.LastGesture = 0
        elif self.gesture == 2: #pull
             self.gestureComplete = 0
             self.yc += self.y_sign*0.03
             if self.yc < -2:
                 self.y_sign = 1
                 self.yc += self.y_sign*0.03
             if self.yc >= -1:
                 self.y_sign = 1
                 self.gestureComplete = 1
                 self.LastGesture = 0
        
        sleep(0.01)
        
    def getGestures(self):
        # Do some TCP stuff here
        #self.device, self.data
        if self.data == None:
            Exception("Cannot read data")
        # print data[-2]
        return self.LastGesture

    def LoadImage( self, file ):
        self.image = Image( )

        try:
            foo = open( file )
    
            self.image.sizeX = foo.size[0]
            self.image.sizeY = foo.size[1]
            self.image.data = foo.tobytes( "raw", "RGBA", 0, -1 )
        except Exception as e:
            print("Could not load image :(")
            print(e)
            sys.exit( )

        return self.image
    
    def InitTexturing( self):

        # create textures
        self.nWoodTexID = glGenTextures( 1 )

        # just use linear filtering
        glBindTexture( GL_TEXTURE_2D, self.nWoodTexID )
        glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR )
        glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR )
        glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT )
        glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT )
        glTexImage2D( GL_TEXTURE_2D, 0, 4,
                      self.oWoodImage.sizeX, self.oWoodImage.sizeY,
                      0, GL_RGBA, GL_UNSIGNED_BYTE, self.oWoodImage.data )


    def ResizeGLScene( self, nWidth, nHeight ):
        # prevent a divide-by-zero error if the window is too small
        if nHeight == 0:
            nHeight = 1
    
        # reset the current viewport and recalculate the perspective transformation
        # for the projection matrix
        glViewport( 0, 0, nWidth, nHeight )
        glMatrixMode( GL_PROJECTION )
        glLoadIdentity( )
        gluPerspective( 45.0, float( nWidth )/float( nHeight ), 0.1, 100.0 )
    
        # return to the modelview matrix mode
        glMatrixMode( GL_MODELVIEW )

#----------------------------------------------------------------------
class RunDemoApp(wx.App):
    def __init__(self):
        wx.App.__init__(self, redirect=False)

    def OnInit(self):
        # style = wx.CLIP_CHILDREN|wx.STAY_ON_TOP|wx.FRAME_SHAPED
        # frame = wx.Frame(None, -1, style=style)
        frame = wx.Frame(None, -1, "GRAPHI Demo",
                        style=wx.DEFAULT_FRAME_STYLE, name="run a sample")
        #frame.CreateStatusBar()

        menuBar = wx.MenuBar()
        menu = wx.Menu()
        item = menu.Append(wx.ID_EXIT, "E&xit\tCtrl-Q", "Exit demo")
        self.Bind(wx.EVT_MENU, self.OnExitApp, item)
        menuBar.Append(menu, "&File")
        
        frame.SetMenuBar(menuBar)
        frame.Show(True)
        frame.Bind(wx.EVT_CLOSE, self.OnCloseFrame)

        win = runTest(frame)

        # set the frame to a good size for showing the two buttons
        frame.SetSize((640,480))
        win.SetFocus()
        self.window = win
        frect = frame.GetRect()

        self.SetTopWindow(frame)
        self.frame = frame
        return True
        
    def OnExitApp(self, evt):
        self.frame.Close(True)

    def OnCloseFrame(self, evt):
        if hasattr(self, "window") and hasattr(self.window, "ShutdownDemo"):
            self.window.ShutdownDemo()
        evt.Skip()

def runTest(frame):
    win = ButtonPanel(frame)
    return win

app = RunDemoApp()
app.MainLoop()
