public class InternalSlot
{
    public Item item;
    public int amount;

    public bool isEmpty
    {
        get
        {
            return amount == 0;
        }
    }

    public bool isFull
    {
        get
        {
            return amount >= item.stackSize;
        }
    }

    public void Empty()
    {
        amount = 0;
    }

    public int AddAmount(int amount)
    {
        this.amount += amount;
        int diff = this.amount - item.stackSize;
        if (diff > 0)
        {
            if (this.amount > item.stackSize)
            {
                this.amount = item.stackSize;
            }
            return diff;
        }
        else
        {
            return 0;
        }
    }

    public InternalSlot()
    {

    }

    public InternalSlot(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}