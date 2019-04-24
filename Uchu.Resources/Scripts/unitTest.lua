--Globals used to check when functions are called
require('State')

calledStartup = false;
globalsWork=true;
calledArrived=false;
calledCancelled=false;

function onStartup(self)
    calledStartup = true;
end

function onArrived(self, msg)
    calledArrived = true;
end

function onCancelled(self,msg)
    calledCancelled = true;
end

-- state machine test
-- Test One
calledTestOneArrived = false;
calledTestOneOnExit = false;
testOne = State.create()
testOne.onArrived = function(self,msg)
    calledTestOneArrived = true;
end

testOne.onExit = function(self,msg)
    calledTestOneOnExit = true;
end


calledTestTwoArrived = false;
calledTestTwoOnEnter = false;
testTwo = State.create()
testTwo.onEnter = function(self)
    calledTestTwoOnEnter = true;
end

testTwo.onArrived = function(self,msg)
    calledTestTwoArrived = true;
end

function TestStateMachine(self)
    print("***************************************************");
    print("Adding states");
    print(self:GetID());
    addState(testOne, "testOne", "testOne", self);
    addState(testTwo, "testTwo", "testTwo", self);
    beginStateMachine("testOne", self);
end

function SetStateToTestTwo(self)
	setState("testTwo", self);
end

function TestFail()
    ThisWillFailNow();
end

function TestInfiniteLoop()
	local i = 0
	while i < 2 do
		i = i
	end
end

function TestRecursionFail()
    TestRecursionFail();
end

