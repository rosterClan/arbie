
using System.Collections;

abstract class eventProvider {
    protected router requestor; 

    public eventProvider() {
        this.requestor = router.Instance;
    }
    abstract public List<eventDTO> execute();
}