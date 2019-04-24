function onStartup(self)

     self:SetVar("Started", false)
     self:SetVar("Time", 1)
     
       
end

function onTimerDone(self,msg)

 	if msg.name =="Count" and self:GetVar("Started") then
 	
		local cTimer = self:GetVar("Time") -1
		self:SetVar("Time", cTimer)
		UI:SendMessage("ChageUI", { {"sgTimer", tostring(cTimer) } })
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "Count", self )
		if cTimer < 0 then
			  GAMEOBJ:GetTimer():CancelAllTimers( self )
			  self:SetVar("Started", false)
			  UI:SendMessage("ChageUI", { {"sgTimer", " " } })
	  		  self:SetVar("Time", GAMEOBJ:GetZoneControlID():GetVar("timelimit"))
		
		end
 	end

end


function onNotifyClientObject(self, msg) 

	if msg.name == "count" then
	     self:SetVar("Time", GAMEOBJ:GetZoneControlID():GetVar("timelimit"))    
		 self:SetVar("Started", true)
		 self:SetVar("TotalTime", msg.parma1)
		 UI:SendMessage("ChageUI", { {"sgTimer", tostring(msg.param1) } })
		 GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "Count", self )
	
	end
	
	if msg.name == "Stop" then
	  GAMEOBJ:GetTimer():CancelAllTimers( self )
	  self:SetVar("Started", false)
	  UI:SendMessage("ChageUI", { {"sgTimer", " " } })
	  self:SetVar("Time", GAMEOBJ:GetZoneControlID():GetVar("timelimit"))
	end
	
	



end
