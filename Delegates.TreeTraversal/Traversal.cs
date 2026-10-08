namespace Delegates.TreeTraversal;

public static class Traversal
{
    private static IEnumerable<TResult> Traverse<TNode, TResult>(
        TNode root,
        Func<TNode, IEnumerable<TNode>> getChildren,
        Func<TNode, bool> isTarget,
        Func<TNode, TResult> selector)
    {
        if (isTarget(root))
            yield return selector(root);

        foreach (var child in getChildren(root))
            foreach (var result in Traverse(child, getChildren, isTarget, selector))
                yield return result;
    }

    public static IEnumerable<Product> GetProducts(ProductCategory root)
    {
        return Traverse(
            root,
            category => category.Categories,
            category => category.Products.Count > 0,
            category => category.Products
        ).SelectMany(products => products);
    }

    public static IEnumerable<Job> GetEndJobs(Job root)
    {
        return Traverse(
            root,
            job => job.Subjobs,
            job => job.Subjobs.Count == 0,
            job => job
        );
    }

    public static IEnumerable<T> GetBinaryTreeValues<T>(BinaryTree<T> root)
    {
        return Traverse(
            root,
            node => new[] { node.Left, node.Right }.Where(n => n != null),
            node => node.Left == null && node.Right == null,
            node => node.Value
        );
    }
}