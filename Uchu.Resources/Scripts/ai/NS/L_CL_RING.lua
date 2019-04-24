-------------------------------------------------------------------
-- script for Lego Club space. Jump through a ring, smash the ring
--
-- created brandi 5/24/10
-- made it an single object, updated the script -ba 5/28/10
-------------------------------------------------------------------

-- player jumpted through the ring
function onCollisionPhantom(self,msg)
 
	-- kill the ring
	self:RequestDie{ killType = "VIOLENT" , killerID = msg.senderID }
	
end
