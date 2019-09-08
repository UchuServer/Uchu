--------------------------------------------------------------------------
--Brick Fury chat bubble text on proximity script
--------------------------------------------------------------------------

require('L_IMAG_BACKPACK_HEALS_CLIENT')

-- Proximity speech
local chat = {Localize("NPC_BRICK_FURY_1"), Localize("NPC_BRICK_FURY_2"), Localize("NPC_BRICK_FURY_3")}

--1 = I love the smell of Maelstrom in the morning!
--2 = You want the Maelstrom?  YOU CAN'T HANDLE THE MAELSTROM!!
--3 = I'm Brick Fury, Mitch!



function onStartup(self)
	self:SetProximityRadius{radius = 30}
	
   self:ShowHealthBar{bShow = false}
end

function onProximityUpdate(self, msg)

    local player = GAMEOBJ:GetControlledID()
    
    if msg.status == "ENTER" and msg.objId:GetID() == player:GetID() then
        
        local chatNum = math.random(1 , #chat)
        
        self:DisplayChatBubble{wsText = chat[chatNum]}
        
    end
end
