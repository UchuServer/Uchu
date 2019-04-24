require('o_mis')
local myPriority = 1

function onStartup(self)

	

end

function onScriptNetworkVarUpdate(self,msg)
 
	if msg.tableOfVars.name == "pushState" then
	
		UI:SendMessage( "pushGameState", {{"state", "dps_gui" }} )
		myPriority = 0
		self:RequestPickTypeUpdate()

		GAMEOBJ:GetTimer():AddTimerWithCancel( 2 , "StorePlayer", self )

	
	elseif msg.tableOfVars.name == "popState" then
	
		UI:SendMessage( "popGameState", {{"state", "dps_gui" }} )
		myPriority = 1
		self:RequestPickTypeUpdate()
		
		
	elseif msg.tableOfVars.name == "TotalHealth" then
	
		UI:SendMessage("DPSGUI", {{"Total", tostring(msg.tableOfVars.int1) }} )
	elseif msg.tableOfVars.name == "StateRunning" then
	
	
		UI:SendMessage("DPSGUI", {{"cstate","running"}} )
	elseif msg.tableOfVars.name == "timer" then
		
		UI:SendMessage("DPSGUI", {{"timer",tostring(msg.tableOfVars.int1)}})
		
	elseif msg.tableOfVars.name == "rest" then
	
		UI:SendMessage("DPSGUI", {{"resetall","1"}})
		
	elseif msg.tableOfVars.name == "Current" then
	
		if msg.tableOfVars.int1 >= 1 then
		    if msg.tableOfVars.String1 then
			UI:SendMessage("DPSGUI", { {"Current", tostring(msg.tableOfVars.int1) }, {"Average", msg.tableOfVars.String1 } } )
			else
			    UI:SendMessage("DPSGUI", { {"Current", tostring(msg.tableOfVars.int1) }} )
			end
		else
		    if msg.tableOfVars.String1 then
			    UI:SendMessage("DPSGUI", {{"Current", "|0" }, {"Average", msg.tableOfVars.string1 } } )
		    else
		         UI:SendMessage("DPSGUI", {{"Current", "|0" }} )
		    end
		end

        if msg.tableOfVars.int2 then
		    UI:SendMessage("DPSGUI", {{"timer",tostring(msg.tableOfVars.int2)}})
	    end
	end
	
end



function onGetPriorityPickListType(self, msg)





    if ( myPriority == 1 ) then

       msg.fCurrentPickTypePriority = myPriority
      
       msg.ePickType = 14   

   else
   		
   		msg.ePickType = -1 
   end

    return msg

end

function onTimerDone(self,msg)


	if msg.name == "StorePlayer" then
	
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		UI:SendMessage("DPSGUI", {{"user", tostring(player) }} )
		UI:SendMessage("DPSGUI", {{"npc", "|"..tostring(self:GetID()) }} )
		
		
	
	end

end
