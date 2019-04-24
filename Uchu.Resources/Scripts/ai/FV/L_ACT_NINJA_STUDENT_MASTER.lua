require('o_mis')
require('State')

function onStartup(self)

    self:SetVar("Student_1", 0 ) 
    self:SetVar("Student_2", 0 ) 
    self:SetVar("Student_3", 0 ) 
    self:SetVar("Student_4", 0 ) 
    self:SetVar("Student_5", 0 )
    self:SetVar("Student_6", 0 )
    self:SetVar("Student_7", 0 )
    
    
    self:SetVar("Sensei_1", 0 ) 
    self:SetVar("Bricks_1", 0 ) 
    
   
    RESMGR:LoadObject { objectTemplate =  2497 , x= 256.084   , y= 270.126  , z= -230.392  , owner = self } -- Student 1
    RESMGR:LoadObject { objectTemplate =  2497 , x= 258.456   , y= 270.127  , z= -238.713  , owner = self } -- Student 2
    RESMGR:LoadObject { objectTemplate =  2497 , x= 268.206   , y= 270.127  , z= -227.332, owner = self } -- Student 3
    RESMGR:LoadObject { objectTemplate =  2497 , x= 270.281   , y= 270.127 , z= -235.269  , owner = self } -- Student 4
    
    
    RESMGR:LoadObject { objectTemplate =  2497 , x= 248.752   , y= 270.463  , z= -200.577 , owner = self } -- Student 5
    RESMGR:LoadObject { objectTemplate =  2497 , x= 258.873   , y= 270.290  , z= -197.299 , owner = self } -- Student 6
    RESMGR:LoadObject { objectTemplate =  2497 , x= 261.941   , y= 270.023  , z= -207.558 , owner = self } -- Student 7

    local config = { {"groupID", "Minimap_MissionGivers"}, {"renderDisabled", false}  }
    RESMGR:LoadObject { objectTemplate =  2498 , x= 201.760  , y= 274.553  , z= -232.331 , owner = self, configData = config  } -- Sensei 
   
    
    
    --RESMGR:LoadObject { objectTemplate =  2499 , x= 197.06  , y= 282.19  , z= -233.25  , owner = self } -- Bricks

  
   
        self:UseStateMachine{} 
         --self:SetPosition{ x= 247, y= 270, z= -199 }
         ------------------------------------------------------ Idle      
        Idle = State.create()
        Idle.onEnter = function(self)
           GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "crane", self )
            
        end 
        Idle.onArrived = function(self)
             
        end 
        
        
        ------------------------------------------------------- Anima 1
        Animaton_1 = State.create()
        Animaton_1.onEnter = function(self)
        
           
      
        
          
        end 
        ------------------------------------------------------- Anima 2
        Animaton_1.onArrived = function(self)
             
        end       
        Animaton_2 = State.create()
        Animaton_2.onEnter = function(self)
          
        end 
        Animaton_2.onArrived = function(self)
             
        end 
       
        
        addState(Animaton_1, "Animaton_1", "Animaton_1", self)
        addState(Animaton_2, "Animaton_2", "Animaton_2", self)
        addState(Idle, "Idle", "Idle", self)
        beginStateMachine("Idle", self) 


end
onChildLoaded = function(self,msg)
       
       if msg.childID:GetLOT().objtemplate == 2497 then
            for i = 1, 7 do  
                if self:GetVar("Student_"..i) == 0 then
                    storePet(self, msg.childID, "Student_" , i )
                  
                    break 
                end 
            end
            msg.childID:SetRotation(self:GetRotation())
       end   
       if msg.childID:GetLOT().objtemplate == 2498 then
               if   self:GetVar("Sensei_1") == 0 then
                storePet(self, msg.childID, "Sensei_" , 1 )
                msg.childID:FaceTarget{location = {x= 207.880, z=-230.764}}
                msg.childID:FaceTarget{location = {x= 207.880, z=-230.764}}
           
                
              

               end
              
       end
      if msg.childID:GetLOT().objtemplate == 2499 then
            if   self:GetVar("Bricks_1") == 0 then
          
               storePet(self, msg.childID, "Bricks_" , 1 )
              setState("Animaton_1",self)

    
           end      
       end 
       msg.childID:SetParentObj{ bSetToSelf = false }
end 

function emote(self,target,skillType)
       
        self:SetVar("EmbeddedTime", self:GetAnimationTime{  animationID = skillType }.time)
        self:PlayFXEffect {priority = 1.2, effectType = skillType}
end           


onTimerDone = function(self, msg)


    if msg.name == "crane" then
         Emote.emote( self, self , "crane" )     
         getPet(self, "Sensei_" , 1 ):SetRotation{ y= 0.51481799602509, x= 0, w=0.8038604259491, z=0   }
        for i = 1, 7 do 
         Emote.emote( getPet(self, "Student_" , i ), getPet(self, "Student_" , i ) , "crane" )     
        end 
         Emote.emote( getPet(self, "Sensei_" , 1 ), getPet(self, "Sensei_" , 1 ) , "crane" ) 
         --Emote.emote( getPet(self, "Bricks_" , 1 ), getPet(self, "Bricks_" , 1 ) , "crane" ) 
         getPet(self, "Sensei_" , 1 ):FaceTarget{location = {x= 253.02, z=-219.15}}
         
         GAMEOBJ:GetTimer():AddTimerWithCancel( 15.33 , "bow", self )
    end
    if msg.name == "bow" then
        Emote.emote( self, self , "bow" )     
        for i = 1, 7 do 
           Emote.emote( getPet(self, "Student_" , i ), getPet(self, "Student_" , i ) , "bow" )     
        end 
        Emote.emote( getPet(self, "Sensei_" , 1 ), getPet(self, "Sensei_" , 1 ) , "bow" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "tiger", self )    
    end
   
    if msg.name == "tiger" then
        Emote.emote( self, self , "tiger" )     
        for i = 1, 7 do 
           Emote.emote( getPet(self, "Student_" , i ), getPet(self, "Student_" , i ) , "tiger" )     
        end 
        Emote.emote( getPet(self, "Sensei_" , 1 ), getPet(self, "Sensei_" , 1 ) , "tiger" ) 
       -- Emote.emote( getPet(self, "Bricks_" , 1 ), getPet(self, "Bricks_" , 1 ) , "tiger" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 15.33 , "bow2", self )      
    end    
    if msg.name == "bow2" then
        Emote.emote( self, self , "bow" )     
        for i = 1, 7 do 
           Emote.emote( getPet(self, "Student_" , i ), getPet(self, "Student_" , i ) , "bow" )     
        end 
        Emote.emote( getPet(self, "Sensei_" , 1 ), getPet(self, "Sensei_" , 1 ) , "bow" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "mantis", self )      
    end    
  
    if msg.name == "mantis" then
        Emote.emote( self, self , "mantis" )     
        for i = 1, 7 do 
           Emote.emote( getPet(self, "Student_" , i ), getPet(self, "Student_" , i ) , "mantis" )     
        end 
        Emote.emote( getPet(self, "Sensei_" , 1 ), getPet(self, "Sensei_" , 1 ) , "mantis" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 15.33, "bow3", self )       
    end
    if msg.name == "bow3" then
        Emote.emote( self, self , "bow" )     
        for i = 1, 7 do 
           Emote.emote( getPet(self, "Student_" , i ), getPet(self, "Student_" , i ) , "bow" )     
        end 
        Emote.emote( getPet(self, "Sensei_" , 1 ), getPet(self, "Sensei_" , 1 ) , "bow" ) 
         --Emote.emote( getPet(self, "Bricks_" , 1 ), getPet(self, "Bricks_" , 1 ) , "mantis" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "repeat", self )      
    end    
    if msg.name == "repeat" then
    
        GAMEOBJ:GetTimer():CancelAllTimers( self )
        setState("Idle", self) 
    
    end 
end 






