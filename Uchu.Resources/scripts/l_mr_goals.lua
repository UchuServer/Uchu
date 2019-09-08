require('o_mis')

CONSTANTS = {}
CONSTANTS["GOAL_LINE_LOT"] = 6283
CONSTANTS["CHECKPOINT_LOT"] = 6281

function onStartup(self) 

    local templateID = self:GetLOT().objtemplate
    local friends = self:GetObjectsInGroup{ group = "MR_Control"}.objects

    for i = 1, table.maxn (friends) do
        friends[i]:NotifyObject{name = "traplabel", ObjIDSender = self}
    end

	-- register with zone control object
	GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
	
end

function onCollisionPhantom(self, msg)

	local faction = msg.objectID:GetFaction()
	
	if faction and faction.faction == 1 then

		local templateID = self:GetLOT().objtemplate
		local strType = ""
		
		if (templateID == CONSTANTS["GOAL_LINE_LOT"]) then
		
   			GAMEOBJ:GetZoneControlID():OffCollisionPhantom{objectID = msg.objectID, senderID = self}
   			
		elseif (templateID == CONSTANTS["CHECKPOINT_LOT"]) then
		
   			GAMEOBJ:GetZoneControlID():CollisionPhantom{objectID = msg.objectID, senderID = self}
		end
			
	end

	return msg
  
end

function onUse(self)
    GAMEOBJ:DeleteObject(self)
end

function onNotifyObject(self, msg)

    local templateID = self:GetLOT().objtemplate

    if msg.name == "Change" then
        if (templateID == CONSTANTS["GOAL_LINE_LOT"]) then
            GAMEOBJ:GetZoneControlID():SetVar("Finishpos_x", self:GetPosition().pos.x)
            GAMEOBJ:GetZoneControlID():SetVar("Finishpos_y", self:GetPosition().pos.y)
            GAMEOBJ:GetZoneControlID():SetVar("Finishpos_z", self:GetPosition().pos.z)
            GAMEOBJ:GetZoneControlID():SetVar("Finishrot_x", self:GetRotation().x)
            GAMEOBJ:GetZoneControlID():SetVar("Finishrot_y", self:GetRotation().y)
            GAMEOBJ:GetZoneControlID():SetVar("Finishrot_z", self:GetRotation().z)
            GAMEOBJ:GetZoneControlID():SetVar("Finishrot_w", self:GetRotation().w)
            GAMEOBJ:GetZoneControlID():SetVar("FinishThere", 1)
        end
    end
end
