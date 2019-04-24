--------------------------------------------------------------
--  client script on the big pipes console
-- when the render component is ready, send a message to the server side script to start the fx
--  
-- created Brandi... 8/25/10
--------------------------------------------------------------

function onRenderComponentReady(self,msg)
	self:FireEventServerSide{ args = "startFX" }
end