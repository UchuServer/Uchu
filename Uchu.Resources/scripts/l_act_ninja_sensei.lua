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
    self:SetVar("Student_8", 0 )  
    self:SetVar("Count", 0 ) 
    self:SetVar("Loaded", false ) 
    -- Student 2497
    
 -----------------------------------------------------------------------------------------------
 -- Create States
 -----------------------------------------------------------------------------------------------
	self:UseStateMachine{}   
	Idle = State.create()
	Idle.onEnter = function(self)
	
		if self:GetVar("Loaded") then
			   GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "crane", self )
			else
			   GAMEOBJ:GetTimer():AddTimerWithCancel( 3 , "loadStudents", self )
		end

	end
	
	Idle.onArrived = function(self)

	end 	
	
	addState(Idle, "Idle", "Idle", self)
	beginStateMachine("Idle", self) 


end

         


onTimerDone = function(self, msg)


	if msg.name == "loadStudents" then
	        local s = 1
			local student = self:GetObjectsInGroup{ group = "Sensei_kids" }.objects
			--print(tostring(table.maxn (student)))
		
		    if table.maxn (student) == 16 then
		        
                for i = 1, table.maxn (student) do 
                    if ( student[i]:GetLOT().objtemplate == 2497 ) then

                        storeObjectByName(self, "Student_"..s  , student[i])
   
                        s = s + 1
                    end 
                   
                end
			
			self:SetVar("Loaded", true ) 
			end	
		
		
		setState("Idle", self ) 
	end


    if msg.name == "crane" then
        for i = 1, 8 do 
         Emote.emote( getObjectByName(self, "Student_"..i ), self , "crane" )     
        end 
         Emote.emote( self, self, "crane" ) 
         GAMEOBJ:GetTimer():AddTimerWithCancel( 15.33 , "bow", self )
    end
    
    if msg.name == "bow" then
        Emote.emote( self, self , "bow" )     
        for i = 1, 8 do 
           Emote.emote( getObjectByName(self, "Student_"..i ), self , "bow" )     
        end 
        Emote.emote( self, self, "bow" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "tiger", self )    
    end
   
    if msg.name == "tiger" then
        Emote.emote( self, self , "tiger" )     
        for i = 1, 8 do 
           Emote.emote( getObjectByName(self, "Student_"..i ), self , "tiger" )     
        end 
        Emote.emote( self, self, "tiger" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 15.33 , "bow2", self )      
    end 
    
    if msg.name == "bow2" then
        Emote.emote( self, self, "bow" )     
        for i = 1, 8 do 
           Emote.emote( getObjectByName(self, "Student_"..i ), self , "bow" )     
        end 
        Emote.emote( self, self, "bow" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "mantis", self )      
    end    
  
    if msg.name == "mantis" then
        Emote.emote( self, self, "mantis" )     
        for i = 1, 8 do 
           Emote.emote( getObjectByName(self, "Student_"..i ), self , "mantis" )     
        end 
        Emote.emote( self, self,  "mantis" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 15.33, "bow3", self )       
    end
    
    if msg.name == "bow3" then
        Emote.emote( self, self, "bow" )     
        for i = 1, 8 do 
           Emote.emote( getObjectByName(self, "Student_"..i ), self , "bow" )     
        end 
        Emote.emote( self, self, "bow" ) 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "repeat", self )      
    end  
    
    if msg.name == "repeat" then
    
        GAMEOBJ:GetTimer():CancelAllTimers( self )
        setState("Idle", self) 
    
    end 
end 






