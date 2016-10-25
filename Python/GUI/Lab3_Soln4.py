#
# COSC3000 - Computer Graphics
# 
# Introduction to OpenGL
#

from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *
from PIL.Image import *
from time import * #Included for Graphics cards that do not have an in-built frame limit (i.e. non-NVIDIA)
from math import * #NOTE: This was included to gain access to the functions sin, cos and pi
from mouseInteractor import MouseInteractor
from DeviceConnect import DeviceConnection

# Used as a global to store the handle of the GLUT window we create.
hWindow = 0

# Global variables for Lab
Phi = 100
Theta = 0
Phi_sign = 1

x = 0.0
y = -1.0
z = 0.0

x_sign = 1
y_sign = 1
z_sign = 1

nWoodTexID = 0
oWoodImage = Image( )
gestureComplete = 1
gestureCounter = 0
gesture = 1

#
# A general OpenGL initialisation function that sets various initial parameters.
# We call this right after out OpenGL window is created.
#
def InitGL( nWidth, nHeight ):
    global quadratic, oWoodImage
    # use black when clearing the colour buffers -- this will give us a black
    # background for the window
    
    # load images
    oWoodImage = LoadImage( "hand-transparent-feather.png" )
    # init texturing
    InitTexturing( )
    
    # use black when clearing the colour buffers -- this will give us a black
    # background for the window
    glClearColor( 0, 0, 0, 0 )
    # enable the depth buffer to be cleared
    glClearDepth( 1.0 )
    # set which type of depth test to use
    glDepthFunc( GL_LESS )
    # enable depth testing
    glEnable( GL_DEPTH_TEST )
    # enable smooth colour shading
    glShadeModel( GL_SMOOTH )
    
    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
    
    ResizeGLScene( nWidth, nHeight )
    
    quadratic = gluNewQuadric()
    gluQuadricNormals(quadratic, GLU_SMOOTH)        # Create Smooth Normals (NEW) 
    gluQuadricTexture(quadratic, GL_TRUE)


#
# Loads the bitmap image, file, for use as a texture
#
def LoadImage( file ):
    image = Image( )

    try:
        foo = open( file )
        
        image.sizeX = foo.size[0]
        image.sizeY = foo.size[1]
        image.data = foo.tobytes( "raw", "RGBA", 0, -1 )
    except Exception as e:
        print("Could not load image :(")
        print(e)
        sys.exit( )

    return image
    
def InitTexturing( ):
    global nWoodTexID, oWoodImage

    # create textures
    nWoodTexID = glGenTextures( 1 )

    # just use linear filtering
    glBindTexture( GL_TEXTURE_2D, nWoodTexID )
    glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR )
    glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR )
    glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT )
    glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT )
    glTexImage2D( GL_TEXTURE_2D, 0, 4,
                  oWoodImage.sizeX, oWoodImage.sizeY,
                  0, GL_RGBA, GL_UNSIGNED_BYTE, oWoodImage.data )

#
# The function called when our window is resized. This won't happen if we run
# in full screen mode.
#
def ResizeGLScene( nWidth, nHeight ):
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

#
# Draw the scene.
#
def DrawGLScene( ):
    global Phi      #<= inform function of global variable
    global Theta    #<= inform function of global variable
    global Phi_sign #<= inform function of global variable
    global quadratic
    global nWoodTexID
    global gestureComplete
    global gesture
    global x, x_sign
    global y, y_sign
    global z, z_sign
    global data
    
    data = device.Read() #read from open TCP connection
    print data[17]
    
    # clear the screen and depth buffer
    glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT )
    # reset the matrix stack with the identity matrix
    glLoadIdentity( )

    
    ''' CODE TO DRAW MODELS STARTS HERE... '''
    
    # move into the screen 6.0 units
    glTranslatef( -0, 0.0, -3 )
    mouseInteractor.applyTransformation( )
     #Perform transforms accourding to mouse input
    glDisable(GL_TEXTURE_2D) #Leave texture mode (if lighting was enabled you would need to do the same for glDisable(GL_LIGHTING))

    #--**Sphere Code**--
    #Make Sphere Green
    # glColor3f(0.0,1.0,0.0)
    #
    # #For detail: see "http://pyopengl.sourceforge.net/documentation/manual-3.0/glutSolidSphere.html"
    # glutSolidSphere( 1 , 100 , 100 )
    # #--**Sphere Code**--
    #
    # #--**Do Some Math**--
    #Compute Origin of Cone and Cylinder Model
    # r_xz = 1.5*cos(Phi*pi/180)
    # y = 1.5*sin(Phi*pi/180)
    # z = r_xz*cos(Theta*pi/180)
    # x = r_xz*sin(Theta*pi/180)
    
    #--**Do Some Math**--

    #--**Cone Code**--
    # glPushMatrix() #Push Stack matrix down by one - orient cone independently of all else
    # #Make Cone Red
    # glColor3f(1.0,0.0,0.0)
    # #Position Cone
    # glTranslatef( x, y, z)
    # #Rotate Cone
    # glRotatef( Theta, 0.0, 1.0, 0.0 )
    # glRotatef( -Phi, 1.0, 0.0, 0.0 )
    # #Scale Cone: Invert Direction of Cone (by default it points away from the origin: see "http://pyopengl.sourceforge.net/documentation/manual-3.0/glutSolidCone.html")
    # glScalef( 1, 1, -1)
    # #Draw Cone
    # glutSolidCone( 0.25 , 0.5 , 10 , 10 )
    # glPopMatrix()
    #--**Cone Code**--
    
    glEnable(GL_TEXTURE_2D)
    
    #--**Cylinder Code**--
    glPushMatrix() #Push Stack matrix down by one - orient cylinder independently of all else
    glBindTexture( GL_TEXTURE_2D, nWoodTexID )
    #Make Cylinder Blue
    #Position Cylinder
    glTranslatef( x, y, z)
    #Rotate Cylinder
    # glRotatef( 135, 0.0, 1.0, 0.0 )
    # glRotatef( Phi*Phi_sign, 0.0, 0.0, 1.0 )
    # glRotatef( -45, 1.0, 0.0, 1.0 )
    
    glRotatef(-135, 1.0, 0.0, 0.0)
    glRotatef(Phi, 0.0, 0.0, 1.0)
    #Scale Cylinder
    #N/A
    #Draw Cylinder
    #glutSolidCylinder(  0.125 , 1 , 10 , 10 )
    glColor4f(1,1,1,1)
    gluCylinder(quadratic,0.4,0.4,1.8,40,40)
    # glBegin( GL_QUADS )
    # glTexCoord2f( 0, 0 ); glVertex3f( 0, 0, 0 )
    # glTexCoord2f( 1, 0 ); glVertex3f( 1, 0, 0 )
    # glTexCoord2f( 1, 1 ); glVertex3f( 1, 1, 0 )
    # glTexCoord2f( 0, 1 ); glVertex3f( 0, 1, 0 )
    # glEnd( )
    glPopMatrix()
    
    # print "Phi:", Phi, "Theta:", Theta, "x:", x, "y:", y, "z", z
    
    # Texture Map
    
    
    
    
    glEnable(GL_BLEND);
    #--**Cylinder Code**--
    
    
    

    ''' ...AND ENDS HERE '''
    
    # since this is double buffered, we need to swap the buffers in order to
    # display what we just drew
    glutSwapBuffers( )

    # increase our rotation value for the triangle
    if gestureComplete:
        gesture = getGestures()
        if gesture == 4:
            Phi_sign = 1
        elif gesture == 3:
            Phi_sign = -1
        elif gesture == 8:
            x_sign = 1
        elif gesture == 7:
            x_sign = -1
        elif gesture == 6:
            z_sign = 1
        elif gesture == 5:
            z_sign = -1
        elif gesture == 1:
            y_sign = 1
        elif gesture == 2:
            y_sign = -1            
    #--**Update Phi**----------------------------------------------------
    if gesture == 0: #relax
        gestureComplete = 1
    elif gesture == 4: # twist out
        gestureComplete = 0
        Phi = Phi + Phi_sign*1
        if Phi > 135:
             Phi_sign = -1
             Phi = Phi + Phi_sign*1.0
        if Phi == 100:
            Phi_sign = 1
            gestureComplete = 1
    elif gesture == 3: #twist in
        gestureComplete = 0
        Phi = Phi + Phi_sign*1
        if Phi < 45:
             Phi_sign = 1
             Phi = Phi + Phi_sign*1.0
        if Phi == 100:
            Phi_sign = 1
            gestureComplete = 1
    elif gesture == 8: #drag right
        gestureComplete = 0
        x += x_sign*0.03
        if x > 1:
            x_sign = -1
            x += x_sign*0.03
        if x <= 0:
            x_sign = 1
            gestureComplete = 1
    elif gesture == 7: #drag left
        gestureComplete = 0
        x += x_sign*0.03
        if x < -1:
            x_sign = 1
            x += x_sign*0.03
        if x >= 0:
            x_sign = 1
            gestureComplete = 1
    elif gesture == 6: #lift
        gestureComplete = 0
        z += z_sign*0.04
        if z > 1.5:
            z_sign = -1
            z += z_sign*0.04
        if z <= 0:
            z_sign = 1
            gestureComplete = 1
    elif gesture == 5: #hit
        gestureComplete = 0
        z += z_sign*0.04
        if z < -1.5:
            z_sign = 1
            z += z_sign*0.04
        if z >= 0:
            z_sign = 1
            gestureComplete = 1
    elif gesture == 1: #push
         gestureComplete = 0
         y += y_sign*0.03
         if y > 0:
             y_sign = -1
             y += y_sign*0.03
         if y <= -1:
             y_sign = 1
             gestureComplete = 1
    elif gesture == 2: #pull
         gestureComplete = 0
         y += y_sign*0.03
         if y < -2:
             y_sign = 1
             y += y_sign*0.03
         if y >= -1:
             y_sign = 1
             gestureComplete = 1      
    
    # Phi = Phi + Phi_sign*1
 #    if Phi > 135:
 #        Phi_sign = -1
 #        Phi = Phi + Phi_sign*1.0
 #    elif Phi < 45:
 #        Phi_sign = 1
 #        Phi = Phi + Phi_sign*1.0
    #--**Update Phi**----------------------------------------------------

    #--**Update Theta**---------------------------------------------------
    # Theta = Theta + 1.0
    # if Theta == 360:
    #     Theta = 0
    #--**Update Theta**---------------------------------------------------

    sleep(0.01) #Included for Graphics cards that do not have an in-built frame limit (i.e. non-NVIDIA)

#
# Process any key presses that occur when the OpenGL window is running.
#
def KeyPressed( key, x, y ):
    key = ord(key)
    
    if key == 27:
        glutDestroyWindow( hWindow )
        sys.exit( )

def getGestures():
    # Do some TCP stuff here
    global device, data
    if data == None:
        Exception("Cannot read data")
    # print data[-2]
    return data[17]
    
    
    # global gestureCounter
    # gestureCounter += 1
    # if gestureCounter%100 == 0:
    #     return 7
    # elif gestureCounter%199 == 0:
    #     gestureCounter = 0
    #     return 8
    # else:
    #     return 0

#
# Main entry point for running the application.
#
def main( ):
    global hWindow, mouseInteractor, device
    
    # initialise GLUT and a few other things
    glutInit( "" )
    glutInitDisplayMode( GLUT_RGBA | GLUT_DOUBLE | GLUT_ALPHA | GLUT_DEPTH )
    glutInitWindowSize( 640, 480 )
    glutInitWindowPosition( 0, 0 )
    
    # create our window
    hWindow = glutCreateWindow( b"MLG Band Calibration Interface" )
    
    # setup the display function callback
    glutDisplayFunc( DrawGLScene )
    
    # Permit Mouse interaction
    mouseInteractor = MouseInteractor( .01, 1 )
    mouseInteractor.registerCallbacks( )
    
    # go full-screen if we want to
    #glutFullScreen( )
    
    # setup the idle function callback -- if we idle, we just want to keep
    # drawing the screen
    glutIdleFunc( DrawGLScene )
    # setup the window resize callback -- this is only needed if we arent going
    # full-screen
    glutReshapeFunc( ResizeGLScene )
    # setup the keyboard function callback to handle key presses
    glutKeyboardFunc( KeyPressed )
    
    device = DeviceConnection()
    device.Connect("127.0.0.1")
    
    # call our init function
    InitGL( 640, 480 )
    
    # enter the window's main loop to set things rolling
    glutMainLoop( )
    device.Disconnect()


# Tell people how to exit, then start the program...
print("To quit: Select OpenGL display window with mouse, then press ESC key.")
main( )
