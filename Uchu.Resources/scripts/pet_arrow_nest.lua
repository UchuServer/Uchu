require('o_mis')

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end


function onClientUse(self, msg)

   local targetID = msg.user 				-- Target OBJ ID 
   targetID:DisplayTooltip{ bShow = true, strText = "To the Nesting Grounds", iTime = 0 }
 
end 
