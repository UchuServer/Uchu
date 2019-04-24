--------------------------------------------------------------
-- Spawned object that collects maelstrom samples
--
-- created mrb... 6/22/11
-- updated abeechler ... 8/30/11 - added client side proximity checking to incorporate visibility check
--------------------------------------------------------------

local checkProx = 12					-- Distance that the sample will collect 
local sampleLOTs = {14718}				-- Lots that can be collected

----------------------------------------------------------------
-- When this object is visible on the client, establish its sense 
-- proximity and pulse immediately
----------------------------------------------------------------
function onRenderComponentReady(self)
	-- Set the proximity for collection
	self:SetProximityRadius{radius = checkProx, name = "checkSample", collisionGroup = 1} 
	-- Pulse the proximity
    self:GetProximityObjects{name = "checkSample"}
end

----------------------------------------------------------------
-- Our object has gotten a proximity hit from a potential valid
-- collection sample - process it accordingly
----------------------------------------------------------------
function onProximityUpdate(self, msg)
	-- Checking to see if we should look for the object
	-- Is it visible?
	local bIsVisible = msg.objId:GetVisible().visible
	
	if((self:GetVar("foundSample")) or (msg.name ~= "checkSample") or 
	   (msg.status ~= "ENTER") or (not msg.objId:Exists()) or (not bIsVisible)) then 
	    -- NOT a valid condition
		return 
	end
	
	-- We have a valid message with a visible object while searching,
	-- now determine if it is the correct object type
	for key, LOT in ipairs(sampleLOTs) do 
	    if msg.objId:GetLOT().objtemplate == LOT then
	        -- We have found a collection sample
	        self:SetVar("foundSample", true)
            -- Notify the server for final object verification and collection			
            self:FireEventServerSide{args = "attemptCollection", senderID = msg.objId}
            break
	    end
	end
end  

----------------------------------------------------------------
-- Receive animation names for client side processing
----------------------------------------------------------------
function onScriptNetworkVarUpdate(self, msg)
    local player = GAMEOBJ:GetControlledID()	
	for varName,varValue in pairs(msg.tableOfVars) do
		-- Check to see if we have the correct message and deal with it
		if varName == "current_anim" then
			-- Play the up animation
			self:PlayAnimation{animationID = varValue, fPriority = 4.0}				end
	end
end 
