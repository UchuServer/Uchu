function onGetPriorityPickListType(self, msg)
   
			local myPriority = 0.8
				
			if ( myPriority > msg.fCurrentPickTypePriority ) then

			   msg.fCurrentPickTypePriority = myPriority
			   msg.ePickType = 14    -- Interactive pick type

			end
		
    return msg
end 

function onStartup(self)
	GAMEOBJ:GetTimer():AddTimerWithCancel(1, "BrickMissionWait", self )
end

function onTimerDone(self, msg)
	if msg.name == "BrickMissionWait" then
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		if player:Exists() then	
			MissionCheck(self);
		else
			GAMEOBJ:GetTimer():AddTimerWithCancel(1, "BrickMissionWait", self )
		end
	end
end

function onNotifyClientObject(self, msg)
	if msg.name == "Pickedup" then
	MissionCheck(self)
	end
end
function MissionCheck(self)
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

	if player:GetMissionState{missionID = 1183}.missionState == 2 then
		
		if player:Exists() then	
			local lootLOT = self:GetVar("Loot")
			if player:GetInvItemCount{ iObjTemplate = lootLOT}.itemCount == 0 then
				self:SetVisible{visible = true, fadeTime = 0}
				self:SetCollisionGroup{colGroup = 1}
			else
				self:SetVisible{visible = false}
				self:SetCollisionGroup{colGroup = 16}
			end
		end
	else
		
		self:SetVisible{visible = false}
		self:SetCollisionGroup{colGroup = 16}
	end
end
