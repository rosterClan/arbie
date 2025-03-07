

database db = database.Instance;
db.SetInnerDatabase(new sqlDatabase());

platformFactory factory = new concretePlatformFactory();

abstractRouter routes = new hydra();
router routerSingleton = router.Instance;
routerSingleton.setRouter(routes);

//eventProvider provider = new sportsbettProvider();
//eventFactory mockFactory = new sportsbettEventFactory();
//factory.createBinding("sportsbet", mockFactory, provider);

eventProvider colProvider = new colossalbetProvider();
eventFactory colFactory = new colossalbetFactory();
factory.createBinding("col", colFactory, colProvider);

eventProvider sbProvider = new sportsbettProvider();
eventFactory sbFactory = new sportsbettEventFactory();
factory.createBinding("sb", sbFactory, sbProvider);

eventProvider bmProvider = new boombetProvider();
eventFactory bmFactory = new boombetFactory();
factory.createBinding("bm", bmFactory, bmProvider);

eventProvider pbProvider = new pointsbetProvider();
eventFactory pbFactory = new pointsbetFactory();
factory.createBinding("bp", pbFactory, pbProvider);

platform colObj = factory.createPlatform("col");
platform sbObj = factory.createPlatform("sb");
platform bmObj = factory.createPlatform("bm");
platform bpObj = factory.createPlatform("bp");

Thread colThread = new Thread(() => colObj.execute());
Thread sbThread = new Thread(() => sbObj.execute());
Thread bmThread = new Thread(() => bmObj.execute());
Thread bpThread = new Thread(() => bpObj.execute());

bpThread.Start();
Thread.Sleep(5000);
colThread.Start();
sbThread.Start();
bmThread.Start();


//platform eventObj = factory.createPlatform("sportsbet");
//eventObj.execute();