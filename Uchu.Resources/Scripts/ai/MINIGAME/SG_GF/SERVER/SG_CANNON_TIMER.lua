function onStartup(self)

     GAMEOBJ:GetZoneControlID():NotifyObject{name = "Cannon_Timer" , ObjIDSender = self }
	 self:SetVar("chargeUI", 100)
	 self:SetVar("charge_count", 10)
	 self:SetVar("charge_keep", 0 )
	 self:SetVar("SC_ACTIVE", false)
	 
	 

end



function onNotifyObject(self,msg) 
	



     --- Start Timer on Client
     if msg.name == "Start" then
        self:SetVar("Started", true)
        local wave = GAMEOBJ:GetZoneControlID():GetVar("ThisWave")
        self:NotifyClientObject{name = "count", param1 = GAMEOBJ:GetZoneControlID():GetVar("timelimit") , paramStr = wave }
        
     end
     -- Stop Timer on Client
     if msg.name == "Stop" then

       self:NotifyClientObject{name = "Stop"}
       
     end
     
     if msg.name == "StartCharge" then
       print(" ********* Timer StartCharge")
        ctimer =  GAMEOBJ:GetZoneControlID():GetVar("CONSTANTS.ChargedTime") / 10
     	GAMEOBJ:GetTimer():AddTimerWithCancel( ctimer , "ChargedCompet", self )
     	self:SetVar("charge_keep", 9 )
     
     end
     
     if msg.name =="ResetCharge" then
  		    print(" ********* Timer ResetCharge")
            GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "ChargedCompet", self )
     end
     

end

function onTimerDone(self,msg)

	if msg.name == "ChargedCompet" then
		 chargeTime = self:GetVar("chargeUI") - 10
		 chargeCount = self:GetVar("charge_count") - 1

		if  GAMEOBJ:GetZoneControlID():GetVar("WaveStatus") then
		             local myParent = self:GetParentObj().objIDParent 
                    if chargeCount == 0 then
                     
                        self:SetVar("charge_keep", 0 )
                        GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "charge_counting", param1 = 999 }
                        self:SetVar("charge_count", 10)
                        self:SetVar("chargeUI", 100 )
             		
             			
             			if  self:GetVar("SC_ACTIVE") then
             			--	print("Send Reset ****************************************")
                        	myParent:NotifyObject{ name = "CannonReg" }
                        	 self:SetVar("SC_ACTIVE", false)
                        end
                        	
                   
                    else
                    	
                    	
                    
                    	
						if not self:GetVar("SC_ACTIVE") then
							-- print("Send Charge ****************************************")
						     myParent:NotifyObject{ name = "CannonSupper" }
                      		 self:SetVar("SC_ACTIVE", true)
							
						end
                    	
                    	
                    	GAMEOBJ:GetZoneControlID():NotifyObject{name = "ResetCharge" }
 
                        
                        self:SetVar("charge_keep", self:GetVar("charge_keep") -1 )
                        GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "charge_counting", param1 = chargeTime }
                        self:SetVar("charge_count", chargeCount )
                        self:SetVar("chargeUI", chargeTime )
                         ctimer =  GAMEOBJ:GetZoneControlID():GetVar("CONSTANTS.ChargedTime") / 10
                        GAMEOBJ:GetTimer():AddTimerWithCancel( ctimer , "ChargedCompet", self )
                    end
                            
                    if self:GetVar("chargeUI") == 0 then
                        
                        self:SetVar("chargeUI", 100)
                    end
            

	        end
	end



end