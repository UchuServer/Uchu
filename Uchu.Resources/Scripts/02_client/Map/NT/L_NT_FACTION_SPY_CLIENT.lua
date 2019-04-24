--------------------------------------------------------------

-- L_NT_FACTION_SPY_CLIENT.lua

-- Catch and process Spy Mission events, including formatting
-- the dialogue script data.
-- Created abeechler ... 4/8/11

--------------------------------------------------------------

----------------------------------------------
-- Server calls to client functionality processed here
----------------------------------------------
function onNotifyClientObject(self, msg)
    baseOnNotifyClientObject(self, msg)
end

function baseOnNotifyClientObject(self, msg)
	
	if(msg.name == "displayDialogueLine") then
		-- Display a chat bubble of localized text over the correct object
		local dialogToken = msg.paramStr
		local dialogTargetObj = msg.paramObj
		
		dialogTargetObj:DisplayChatBubble{wsText = Localize(dialogToken)} 
	end
	
end
