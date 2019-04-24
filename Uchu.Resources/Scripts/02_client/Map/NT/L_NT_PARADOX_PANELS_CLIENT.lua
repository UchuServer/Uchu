------------------------------------------------------
-- Script for Paradox Panels
-- This Script controls the power panels idle spark so that once the panel is fixed it will no longer spark
--
-- Made by Ray 1/18/11
-- Updated: mrb... 2/22/11
------------------------------------------------------

-- Does the spark check function on startup
function onRenderComponentReady(self, msg) 
	SparkCheck(self)
end

function onCheckUseRequirements(self, msg)
	local flag = self:GetVar('flag')
	
	if msg.objIDUser:GetFlag{iFlagID = flag}.bFlag or self:GetVar("bHide") == 1 then
		msg.bCanUse = false
	end --flag check
	
	return msg
end

--Custom Function to shut the render component off if that panel has been repaired so the shift icon doesnt appear
function SparkCheck(self)
	local flag = self:GetVar('flag')
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	
	if player:GetFlag{iFlagID = flag}.bFlag then 
		self:SetVisible{visible = false, fadeTime = 0}
		self:SetVar("bHide", 1)
		self:RequestPickTypeUpdate()
	end
end

function onNotifyClientObject(self, msg) 
	--Does the spark check when told to by the server
	if msg.name == "SparkStop" then  
		SparkCheck(self)
		msg.paramObj:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
	elseif msg.name == "bActive" then		
		self:SetVar("bHide", msg.param1)
		self:RequestPickTypeUpdate()
		msg.paramObj:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
	end
end

function onGetPriorityPickListType(self, msg)
	if self:GetVar("bHide") == 1 then 
		msg.ePickType = -1
	else		
		local myPriority = 0.8
			
		if ( myPriority > msg.fCurrentPickTypePriority ) then
		   msg.fCurrentPickTypePriority = myPriority
		   msg.ePickType = 14    -- Interactive pick type
		end
	end
	
    return msg
end 