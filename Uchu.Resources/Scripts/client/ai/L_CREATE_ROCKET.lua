require('o_mis')

function onCollisionPhantom(self, msg)
	if GAMEOBJ:GetLocalCharID() == msg.senderID:GetID() then
        print "CollisionPhantom"
	    storeObjectByName(self, "Rocketbuilder", msg.senderID)

	    print "is local char"
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

        for i =0, player:GetInventorySize{inventoryType = 1 }.size  do
            if player:GetInventoryItemInSlot{slot = i }.itemID:Exists() then
                if player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate >= 4684 
                and player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate <=4721 then
                   local item = player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate
                   self:SetVar("rocketLOT", item)
                   print ("Found rocket " .. item .. "in slot " .. i)

                   RESMGR:LoadObject { objectTemplate =  item , owner = self }

                   break
                end
            end
        end
	end

end

function onStartup (self)
    print "Create Rocket Starting"
end

function onChildLoaded(self, msg)
    print "ChildLoaded"	
	    if msg.templateID == self:GetVar("rocketLOT") then
	        storeObjectByName(self,"rocketID",msg.childID)
	    end
end



